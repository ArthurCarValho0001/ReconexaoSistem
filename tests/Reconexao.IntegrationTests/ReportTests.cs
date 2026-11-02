using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Npgsql;
using Reconexao.Api;
using Xunit;

namespace Reconexao.IntegrationTests;

public sealed class ReportTests : IAsyncLifetime
{
    private string _connection = "";
    private ReportFactory _factory = null!;
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "reconexao-report-test-" + Guid.NewGuid().ToString("N"));
    private readonly string _password = "Ficticia-" + Guid.NewGuid().ToString("N") + "aA1!";
    private readonly List<AppUser> _users = [];
    private readonly CapturedLogs _logs = new();

    public async Task InitializeAsync()
    {
        _connection = Environment.GetEnvironmentVariable("RECONEXAO_TEST_CONNECTION") ?? throw new InvalidOperationException("Use node scripts/local.mjs test.");
        var target = new NpgsqlConnectionStringBuilder(_connection);
        if (target.Host != "127.0.0.1" || target.Port != 55433 || target.Database != "reconexao_test" || target.Username != "reconexao_test_app")
            throw new InvalidOperationException("Testes de relatos somente no banco descartável isolado.");
        _factory = new ReportFactory(_connection, _directory, _logs);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
        await db.Database.MigrateAsync();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        foreach (var role in new[] { "aluno", "aluno", "admin", "admin" })
        {
            var address = $"relato-{Guid.NewGuid():N}@example.test";
            var user = new AppUser { UserName = address, Email = address, Role = role };
            Assert.True((await manager.CreateAsync(user, _password)).Succeeded, "Conta fictícia do teste não criada.");
            _users.Add(user);
        }
    }

    public async Task DisposeAsync()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
            var ids = _users.Select(user => user.Id).ToList();
            await db.Reports.Where(report => ids.Contains(report.AuthorId)).ExecuteDeleteAsync();
            await db.Users.Where(user => ids.Contains(user.Id)).ExecuteDeleteAsync();
        }
        await _factory.DisposeAsync();
        var full = Path.GetFullPath(_directory);
        var temporary = Path.GetFullPath(Path.GetTempPath());
        if (!full.StartsWith(temporary, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Diretório de teste inesperado.");
        if (Directory.Exists(full)) Directory.Delete(full, true);
    }

    private HttpClient Client(WebApplicationFactory<Program>? factory = null) => (factory ?? _factory).CreateClient(
        new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
    private static async Task<HttpResponseMessage> Post(HttpClient client, string route, object body)
    {
        var token = await client.GetFromJsonAsync<JsonElement>("/api/auth/csrf");
        using var request = new HttpRequestMessage(HttpMethod.Post, route) { Content = JsonContent.Create(body) };
        request.Headers.Add("X-CSRF-TOKEN", token.GetProperty("token").GetString());
        return await client.SendAsync(request);
    }
    private async Task Login(HttpClient client, int user = 0) =>
        Assert.Equal(HttpStatusCode.OK, (await Post(client, "/api/auth/login", new { email = _users[user].Email, password = _password })).StatusCode);
    private async Task<List<PrivateReport>> OwnReports()
    {
        using var scope = _factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<FoundationDbContext>().Reports.AsNoTracking().Where(report => report.AuthorId == _users[0].Id).ToListAsync();
    }
    private void AssertNotLogged(string text) => Assert.True(!_logs.Messages.Any(message => message.Contains(text, StringComparison.Ordinal)), "Conteúdo de relato apareceu em log.");

    [Theory]
    [InlineData("ascii", 1000, true)]
    [InlineData("ascii", 1001, false)]
    [InlineData("emoji", 1000, true)]
    [InlineData("emoji", 1001, false)]
    [InlineData("combining", 500, true)]
    [InlineData("combining", 501, false)]
    public async Task UnicodeLimitsAreScalarsNotUtf16(string kind, int repetitions, bool valid)
    {
        var unit = kind switch { "emoji" => "😀", "combining" => "e\u0301", _ => "a" };
        var text = string.Concat(Enumerable.Repeat(unit, repetitions));
        using var client = Client(); await Login(client);
        var response = await Post(client, "/api/relatos", new { text });
        Assert.Equal(valid ? HttpStatusCode.Created : HttpStatusCode.BadRequest, response.StatusCode);
        var rows = await OwnReports();
        Assert.Equal(valid ? 1 : 0, rows.Count);
        if (valid) Assert.True(rows[0].Text == text, "Conteúdo Unicode armazenado divergente.");
        AssertNotLogged(text);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" \t\n\r\u0085\u00a0\u2003\u2028\u3000")]
    [InlineData("texto\0inválido")]
    public async Task BlankAndNullCodePointAreRejected(string text)
    {
        using var client = Client(); await Login(client);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/api/relatos", new { text })).StatusCode);
        Assert.Empty(await OwnReports());
    }

    [Fact]
    public async Task InvalidSurrogateAndMalformedBodyReturnGenericValidation()
    {
        using var client = Client(); await Login(client);
        foreach (var json in new[] { "{\"text\":\"\\ud800\"}", "{\"text\":\"\\udc00\"}", "{", "null", "{}", "[]" })
        {
            var csrf = await client.GetFromJsonAsync<JsonElement>("/api/auth/csrf");
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/relatos") { Content = new StringContent(json, Encoding.UTF8, "application/json") };
            request.Headers.Add("X-CSRF-TOKEN", csrf.GetProperty("token").GetString());
            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        Assert.Empty(await OwnReports());
    }

    [Fact]
    public async Task NewlineNormalizationAndFamilyEmojiHaveExplicitCounts()
    {
        using var client = Client(); await Login(client);
        var lines = "a" + string.Concat(Enumerable.Repeat("\r\n", 998)) + "\r";
        Assert.Equal(HttpStatusCode.Created, (await Post(client, "/api/relatos", new { text = lines })).StatusCode);
        Assert.True((await OwnReports()).Single().Text == "a" + new string('\n', 999), "Quebras de linha não normalizadas.");
        var family = string.Concat(Enumerable.Repeat("👨‍👩‍👧‍👦", 142)) + "123456";
        Assert.Equal(HttpStatusCode.Created, (await Post(client, "/api/relatos", new { text = family })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/api/relatos", new { text = family + "x" })).StatusCode);
    }

    [Fact]
    public async Task SessionOwnsAuthorAndServerDefinesUtcWithoutEchoOrPoints()
    {
        var text = "Relato fictício privado " + Guid.NewGuid().ToString("N");
        using var client = Client(); await Login(client);
        var before = DateTimeOffset.UtcNow.AddMilliseconds(-1);
        var response = await Post(client, "/api/relatos", new { text });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("received", body.GetProperty("status").GetString());
        Assert.Single(body.EnumerateObject());
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
        var report = (await OwnReports()).Single();
        Assert.Equal(_users[0].Id, report.AuthorId);
        Assert.InRange(report.CreatedAt, before, DateTimeOffset.UtcNow);
        Assert.Equal(TimeSpan.Zero, report.CreatedAt.Offset);
        Assert.True(report.Text == text, "Texto do relato divergente.");
        AssertNotLogged(text);
    }

    [Fact]
    public async Task ForgedAuthorAndDateAreRejectedWithoutEchoOrLogs()
    {
        var text = "Relato fictício sigiloso " + Guid.NewGuid().ToString("N");
        using var client = Client(); await Login(client);
        foreach (var body in new object[] { new { text, authorId = _users[1].Id }, new { text, createdAt = "2000-01-01T00:00:00Z" } })
        {
            var response = await Post(client, "/api/relatos", body);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(!(await response.Content.ReadAsStringAsync()).Contains(text, StringComparison.Ordinal), "Erro ecoou relato.");
        }
        Assert.Empty(await OwnReports());
        AssertNotLogged(text);
        AssertNotLogged(_users[1].Id);
    }

    [Fact]
    public async Task AnonymousAndStudentsCannotReadHistoryOrAdminRoutes()
    {
        using var anonymous = Client();
        Assert.Equal(HttpStatusCode.Unauthorized, (await Post(anonymous, "/api/relatos", new { text = "fictício" })).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/api/admin/relatos")).StatusCode);
        using var student = Client(); await Login(student);
        Assert.Equal(HttpStatusCode.Created, (await Post(student, "/api/relatos", new { text = "Somente teste fictício" })).StatusCode);
        var id = (await OwnReports()).Single().Id;
        foreach (var route in new[] { "/api/admin/relatos", $"/api/admin/relatos?authorId={_users[1].Id}", $"/api/admin/relatos/{id}" })
        {
            var response = await student.GetAsync(route);
            Assert.True(response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound, "Aluno acessou rota administrativa.");
        }
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await student.GetAsync("/api/relatos")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await student.GetAsync($"/api/relatos/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await student.GetAsync($"/api/alunos/{_users[1].Id}/relatos")).StatusCode);
        using var admin = Client(); await Login(admin, 2);
        Assert.Equal(HttpStatusCode.Forbidden, (await Post(admin, "/api/relatos", new { text = "fictício" })).StatusCode);
    }

    [Fact]
    public async Task CsrfAndSessionRevocationAreEnforcedOnSubmit()
    {
        using var first = Client(); await Login(first);
        Assert.Equal(HttpStatusCode.BadRequest, (await first.PostAsJsonAsync("/api/relatos", new { text = "fictício" })).StatusCode);
        using var second = Client(); await Login(second);
        Assert.Equal(HttpStatusCode.Unauthorized, (await Post(first, "/api/relatos", new { text = "fictício" })).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await Post(second, "/api/relatos", new { text = "fictício" })).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await Post(second, "/api/auth/logout", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await Post(second, "/api/relatos", new { text = "fictício" })).StatusCode);
        Assert.Single(await OwnReports());
    }

    [Fact]
    public async Task AllAdminsCanReadAuthorDateAndRawTextWithStableSeekPagination()
    {
        var instant = new DateTimeOffset(2035, 1, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(123450);
        var text = "<script>window.t03 = true</script> Relato fictício " + Guid.NewGuid().ToString("N");
        List<string> expected;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
            for (var i = 0; i < 5; i++) db.Reports.Add(new PrivateReport { AuthorId = _users[i % 2].Id, Text = text + i, CreatedAt = instant });
            await db.SaveChangesAsync();
            expected = await db.Reports.Where(report => report.AuthorId == _users[0].Id || report.AuthorId == _users[1].Id)
                .OrderByDescending(report => report.Id).Select(report => report.Id.ToString()).ToListAsync();
        }
        using var admin = Client(); await Login(admin, 2);
        var first = await admin.GetFromJsonAsync<JsonElement>("/api/admin/relatos?limit=2");
        var received = first.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("id").GetString()!).ToList();
        var item = first.GetProperty("items")[0];
        Assert.True(item.GetProperty("text").GetString()!.StartsWith(text, StringComparison.Ordinal), "Texto privado da consulta divergente.");
        Assert.True(_users.Take(2).Any(user => user.Email == item.GetProperty("author").GetProperty("email").GetString()), "Autor da consulta divergente.");
        Assert.Equal(instant, item.GetProperty("createdAt").GetDateTimeOffset());
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
            db.Reports.Add(new PrivateReport { AuthorId = _users[0].Id, Text = "Novo fictício após primeira página", CreatedAt = instant.AddDays(1) });
            await db.SaveChangesAsync();
        }
        var cursor = first.GetProperty("nextCursor").GetString();
        while (cursor is not null)
        {
            var page = await admin.GetFromJsonAsync<JsonElement>("/api/admin/relatos?limit=2&cursor=" + Uri.EscapeDataString(cursor));
            received.AddRange(page.GetProperty("items").EnumerateArray().Select(row => row.GetProperty("id").GetString()!));
            cursor = page.GetProperty("nextCursor").GetString();
        }
        Assert.Equal(received.Count, received.Distinct().Count());
        Assert.True(received.Where(expected.Contains).SequenceEqual(expected), "Desempate de paginação instável ou relato omitido.");
        using var anotherAdmin = Client(); await Login(anotherAdmin, 3);
        Assert.Equal(HttpStatusCode.OK, (await anotherAdmin.GetAsync("/api/admin/relatos?limit=1")).StatusCode);
        AssertNotLogged(text);
    }

    [Theory]
    [InlineData("limit=0")]
    [InlineData("limit=51")]
    [InlineData("limit=abc")]
    [InlineData("limit=2&limit=3")]
    [InlineData("cursor=inválido")]
    [InlineData("cursor=")]
    public async Task InvalidPaginationIsRejected(string query)
    {
        using var client = Client(); await Login(client, 2);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/admin/relatos?" + query)).StatusCode);
    }

    [Theory]
    [InlineData("0", "2035-01-01T00:00:00Z")]
    [InlineData("9223372036854775808", "2035-01-01T00:00:00Z")]
    [InlineData("1", "2035-01-01T00:00:00+03:00")]
    [InlineData("1", "data-inválida")]
    public async Task InvalidCursorIdAndTimestampAreRejected(string id, string createdAt)
    {
        var cursor = WebEncoders.Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new { id, createdAt }));
        using var client = Client(); await Login(client, 2);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/admin/relatos?cursor=" + cursor)).StatusCode);
    }

    [Fact]
    public async Task DatabaseFailureDoesNotEchoOrLogReport()
    {
        var text = "Relato fictício para falha " + Guid.NewGuid().ToString("N");
        await using var failing = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            services.AddDbContext<FoundationDbContext>(options => options.AddInterceptors(new FailedReportWrite(text)))));
        using var client = Client(failing); await Login(client);
        var response = await Post(client, "/api/relatos", new { text });
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.True(!(await response.Content.ReadAsStringAsync()).Contains(text, StringComparison.Ordinal), "Erro de banco ecoou relato.");
        Assert.Empty(await OwnReports());
        AssertNotLogged(text);
    }

    [Fact]
    public async Task DatabaseLengthConstraintCountsEmojiAsOneCharacter()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
        var text = string.Concat(Enumerable.Repeat("😀", 1000));
        db.Reports.Add(new PrivateReport { AuthorId = _users[0].Id, Text = text, CreatedAt = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync();
        db.Reports.Add(new PrivateReport { AuthorId = _users[0].Id, Text = text + "x", CreatedAt = DateTimeOffset.UtcNow });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    private sealed class ReportFactory(string connection, string directory, CapturedLogs logs) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.UseSetting("ConnectionStrings:Database", connection);
            builder.UseSetting("Auth:KeysPath", Path.Combine(directory, "keys"));
            builder.ConfigureLogging(logging => { logging.ClearProviders(); logging.AddProvider(logs); });
        }
    }
    private sealed class CapturedLogs : ILoggerProvider
    {
        public ConcurrentQueue<string> Messages { get; } = new();
        public ILogger CreateLogger(string categoryName) => new CapturedLogger(Messages);
        public void Dispose() { }
        private sealed class CapturedLogger(ConcurrentQueue<string> messages) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
                messages.Enqueue(formatter(state, exception) + exception?.ToString());
        }
    }
    private sealed class FailedReportWrite(string text) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context!.ChangeTracker.Entries<PrivateReport>().Any(entry => entry.State == EntityState.Added))
                throw new DbUpdateException("Falha fictícia de persistência.", new NpgsqlException(text));
            return ValueTask.FromResult(result);
        }
    }
}
