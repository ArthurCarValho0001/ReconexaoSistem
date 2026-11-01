using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Reconexao.Api;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Reconexao.IntegrationTests;

public sealed class AuthTests : IAsyncLifetime
{
    private AuthFactory _factory = null!;
    private string _email = "";
    private readonly string _password = "Ficticia-" + Guid.NewGuid().ToString("N") + "aA1!";
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "reconexao-auth-test-" + Guid.NewGuid().ToString("N"));

    public async Task InitializeAsync()
    {
        var connection = Environment.GetEnvironmentVariable("RECONEXAO_TEST_CONNECTION")
            ?? throw new InvalidOperationException("Execute node scripts/local.mjs test.");
        var settings = new NpgsqlConnectionStringBuilder(connection);
        if (settings.Host != "127.0.0.1" || settings.Port != 55433 || settings.Database != "reconexao_test" || settings.Username != "reconexao_test_app")
            throw new InvalidOperationException("Somente banco descartável isolado.");
        _factory = new AuthFactory(connection, _directory);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
        await db.Database.MigrateAsync();
        _email = $"aluno-{Guid.NewGuid():N}@example.test";
        var result = await scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>().CreateAsync(
            new AppUser { UserName = _email, Email = _email }, _password);
        Assert.True(result.Succeeded, "Criação de conta fictícia de teste falhou.");
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        // Arquivos ficam somente no diretório temporário isolado, nunca em evidências/logs.
        if (Directory.Exists(_directory)) Directory.Delete(_directory, true);
    }

    private HttpClient Client() => _factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
    private static async Task<HttpResponseMessage> Post(HttpClient client, string route, object input)
    {
        var token = await client.GetFromJsonAsync<JsonElement>("/api/auth/csrf");
        using var request = new HttpRequestMessage(HttpMethod.Post, route) { Content = JsonContent.Create(input) };
        request.Headers.Add("X-CSRF-TOKEN", token.GetProperty("token").GetString());
        return await client.SendAsync(request);
    }
    private Task<HttpResponseMessage> Login(HttpClient client) => Post(client, "/api/auth/login", new { email = _email, password = _password });
    private async Task<string> Recovery(HttpClient client)
    {
        Assert.Equal(HttpStatusCode.Accepted, (await Post(client, "/api/auth/forgot-password", new { email = _email })).StatusCode);
        var file = Directory.GetFiles(Path.Combine(_directory, "emails"), "*.json").Single();
        using var message = JsonDocument.Parse(await File.ReadAllTextAsync(file));
        return message.RootElement.GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task LoginRotatesAndLogoutRevokesIncludingCookieReplay()
    {
        using var first = Client(); using var second = Client();
        var response = await Login(first);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cookie = response.Headers.GetValues("Set-Cookie").Single(value => value.StartsWith("__Host-Reconexao="));
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=strict", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(HttpStatusCode.OK, (await Login(second)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await first.GetAsync("/api/auth/session")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await second.GetAsync("/api/auth/session")).StatusCode);
        var login = await Login(second);
        var replayCookie = login.Headers.GetValues("Set-Cookie").Single(value => value.StartsWith("__Host-Reconexao=")).Split(';')[0];
        Assert.Equal(HttpStatusCode.NoContent, (await Post(second, "/api/auth/logout", new { })).StatusCode);
        using var replay = _factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), HandleCookies = false });
        replay.DefaultRequestHeaders.Add("Cookie", replayCookie);
        Assert.Equal(HttpStatusCode.Unauthorized, (await replay.GetAsync("/api/auth/session")).StatusCode);
    }

    [Fact]
    public async Task ConcurrentLoginsLeaveExactlyOneValidSession()
    {
        using var first = Client(); using var second = Client();
        var responses = await Task.WhenAll(Login(first), Login(second));
        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
        var statuses = await Task.WhenAll(first.GetAsync("/api/auth/session"), second.GetAsync("/api/auth/session"));
        Assert.Single(statuses, response => response.StatusCode == HttpStatusCode.OK);
        Assert.Single(statuses, response => response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RecoveryRequestPreservesSessionAndResetRevokesAndCannotReplay()
    {
        using var client = Client();
        await Login(client);
        var token = await Recovery(client);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/auth/session")).StatusCode);
        var nextPassword = _password + "Novo";
        Assert.Equal(HttpStatusCode.NoContent, (await Post(client, "/api/auth/reset-password", new { email = _email, token, password = nextPassword })).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/auth/session")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/api/auth/reset-password", new { email = _email, token, password = nextPassword })).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await Login(client)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Post(client, "/api/auth/login", new { email = _email, password = nextPassword })).StatusCode);
    }

    [Fact]
    public async Task ConcurrentResetsConsumeTokenOnlyOnce()
    {
        using var first = Client(); using var second = Client();
        var token = await Recovery(first);
        var results = await Task.WhenAll(Post(first, "/api/auth/reset-password", new { email = _email, token, password = _password + "Novo" }),
            Post(second, "/api/auth/reset-password", new { email = _email, token, password = _password + "Outro" }));
        Assert.Single(results, response => response.StatusCode == HttpStatusCode.NoContent);
        Assert.Single(results, response => response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidAndExpiredTokensDoNotRevokeSession()
    {
        using var client = Client(); await Login(client);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/api/auth/reset-password", new { email = _email, token = "invalido", password = _password })).StatusCode);
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await manager.FindByEmailAsync(_email);
        var options = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<DataProtectionTokenProviderOptions>>();
        var before = options.Value.TokenLifespan;
        options.Value.TokenLifespan = TimeSpan.FromMinutes(-1);
        var token = await manager.GeneratePasswordResetTokenAsync(user!);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/api/auth/reset-password", new { email = _email, token, password = _password })).StatusCode);
        options.Value.TokenLifespan = before;
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/auth/session")).StatusCode);
    }

    [Fact]
    public async Task AnonymousAndStudentCannotEscalateOrReadOtherUser()
    {
        string otherId;
        using (var scope = _factory.Services.CreateScope())
        {
            var other = new AppUser { UserName = $"b-{Guid.NewGuid():N}@example.test", Email = $"b-{Guid.NewGuid():N}@example.test" };
            Assert.True((await scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>().CreateAsync(other, _password)).Succeeded);
            otherId = other.Id;
        }
        using var client = Client();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/auth/session")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/admin/session")).StatusCode);
        var login = await Post(client, "/api/auth/login", new { email = _email, password = _password, role = "admin", alunoId = "outro" });
        var profile = await login.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("aluno", profile.GetProperty("role").GetString());
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/admin/session")).StatusCode);
        var session = await client.GetFromJsonAsync<JsonElement>($"/api/auth/session?alunoId={otherId}");
        Assert.Equal(_email, session.GetProperty("email").GetString());
        Assert.NotEqual(otherId, session.GetProperty("id").GetString());
        Assert.False(session.TryGetProperty("googleVerified", out _));
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/auth/users/outro")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Post(client, "/api/auth/register", new { email = _email, password = _password, role = "admin" })).StatusCode);
    }

    [Fact]
    public async Task AdminAccessIsGrantedFromDatabaseOnlyAndRoleChangeInvalidatesCookie()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
            await db.Users.Where(user => user.Email == _email).ExecuteUpdateAsync(update => update.SetProperty(user => user.Role, "admin"));
        }
        using var client = Client(); await Login(client);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/admin/session")).StatusCode);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
            await db.Users.Where(user => user.Email == _email).ExecuteUpdateAsync(update => update.SetProperty(user => user.Role, "aluno"));
        }
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/admin/session")).StatusCode);
    }

    [Fact]
    public async Task AllWritesRequireCsrfAndBadCredentialsAreGeneric()
    {
        using var client = Client();
        foreach (var endpoint in new[] { "login", "forgot-password", "reset-password" })
            Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/auth/" + endpoint, new { email = _email, password = _password, token = "x" })).StatusCode);
        var missing = await Post(client, "/api/auth/login", new { email = "missing@example.test", password = _password });
        var wrong = await Post(client, "/api/auth/login", new { email = _email, password = "incorreta" });
        Assert.Equal(HttpStatusCode.Unauthorized, missing.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, wrong.StatusCode);
        var missingBody = await missing.Content.ReadFromJsonAsync<JsonElement>();
        var wrongBody = await wrong.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(missingBody.GetProperty("title").GetString(), wrongBody.GetProperty("title").GetString());
        await Login(client);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/auth/logout", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/auth/session")).StatusCode);
    }

    [Fact]
    public async Task LocalEmailAdapterCannotStartInProduction()
    {
        using var factory = new AuthFactory(Environment.GetEnvironmentVariable("RECONEXAO_TEST_CONNECTION")!, _directory, "Production");
        Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
        await Task.CompletedTask;
    }

    [Fact]
    public async Task LockoutAndRateLimitRestrictRepeatedFailures()
    {
        using var client = Client();
        for (var i = 0; i < 5; i++)
            Assert.Equal(HttpStatusCode.Unauthorized, (await Post(client, "/api/auth/login", new { email = _email, password = "incorreta" })).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await Login(client)).StatusCode);
        HttpResponseMessage response = null!;
        for (var i = 0; i < 20; i++) response = await Post(client, "/api/auth/forgot-password", new { email = "missing@example.test" });
        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }

    [Fact]
    public async Task BrokenEmailDirectoryIsUnavailableForBothKnownAndUnknownEmails()
    {
        var directory = Path.Combine(_directory, "emails");
        await File.WriteAllTextAsync(directory, "fixture");
        using var client = Client();
        Assert.Equal(HttpStatusCode.ServiceUnavailable, (await Post(client, "/api/auth/forgot-password", new { email = _email })).StatusCode);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, (await Post(client, "/api/auth/forgot-password", new { email = "missing@example.test" })).StatusCode);
    }

    [Fact]
    public async Task MissingEmailAdapterReturnsUnavailableAndUnknownEmailDoesNotEnumerate()
    {
        using var client = Client();
        Assert.Equal(HttpStatusCode.Accepted, (await Post(client, "/api/auth/forgot-password", new { email = "missing@example.test" })).StatusCode);
        Assert.Empty(Directory.GetFiles(Path.Combine(_directory, "emails")));
        using var factory = new AuthFactory(Environment.GetEnvironmentVariable("RECONEXAO_TEST_CONNECTION")!, _directory, adapter: "Disabled");
        using var disabled = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        Assert.Equal(HttpStatusCode.ServiceUnavailable, (await Post(disabled, "/api/auth/forgot-password", new { email = _email })).StatusCode);
    }

    private sealed class AuthFactory(string connection, string directory, string environment = "Test", string adapter = "Local") : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(environment);
            builder.UseSetting("ConnectionStrings:Database", connection);
            builder.UseSetting("Auth:KeysPath", Path.Combine(directory, "keys"));
            builder.UseSetting("Auth:EmailAdapter", adapter);
            builder.UseSetting("Auth:LocalEmailPath", Path.Combine(directory, "emails"));
            builder.ConfigureLogging(logging => logging.ClearProviders());
        }
    }

    [Fact]
    public async Task DeliveryFailureAfterProbeDoesNotEnumerateOrRevoke()
    {
        await using var factory = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IRecoveryEmail>();
            services.AddSingleton<IRecoveryEmail, FailingEmail>();
        }));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        await Login(client);
        Assert.Equal(HttpStatusCode.Accepted, (await Post(client, "/api/auth/forgot-password", new { email = _email })).StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, (await Post(client, "/api/auth/forgot-password", new { email = "missing@example.test" })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/auth/session")).StatusCode);
    }

    [Fact]
    public async Task SeedIsExplicitIdempotentAndBootstrapCannotPromoteExistingAccount()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Test", Args = [] });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Auth:KeysPath"] = Path.Combine(_directory, "bootstrap-keys"),
            ["LocalAuth:Password"] = _password,
            ["Bootstrap:Email"] = _email,
            ["Bootstrap:Password"] = _password
        });
        builder.Logging.ClearProviders();
        builder.Services.AddDbContext<FoundationDbContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("RECONEXAO_TEST_CONNECTION")));
        builder.AddAuth();
        await using var app = builder.Build();
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
        var fixtureEmails = new[] { "admin@example.test", "aluno.a@example.test", "aluno.b@example.test" };
        var previousIds = await db.Users.Where(user => fixtureEmails.Contains(user.Email!)).Select(user => user.Id).ToListAsync();
        var previousB = await db.Users.CountAsync(user => user.Email == "aluno.b@example.test");
        await Auth.Bootstrap(app, true, demo: true);
        var demoBefore = await db.Users.AsNoTracking().Where(user => user.Email == "admin@example.test" || user.Email == "aluno.a@example.test")
            .OrderBy(user => user.Email).Select(user => new { user.Id, user.Role, user.PasswordHash }).ToListAsync();
        Assert.Equal(2, demoBefore.Count);
        Assert.Contains(demoBefore, user => user.Role == "admin");
        Assert.Contains(demoBefore, user => user.Role == "aluno");
        await Auth.Bootstrap(app, true, demo: true);
        var demoAfter = await db.Users.AsNoTracking().Where(user => user.Email == "admin@example.test" || user.Email == "aluno.a@example.test")
            .OrderBy(user => user.Email).Select(user => new { user.Id, user.Role, user.PasswordHash }).ToListAsync();
        Assert.True(demoBefore.SequenceEqual(demoAfter), "Demonstração não deve recriar contas ou redefinir senhas.");
        Assert.Equal(previousB, await db.Users.CountAsync(user => user.Email == "aluno.b@example.test"));
        await Auth.Bootstrap(app, true);
        await Auth.Bootstrap(app, true);
        await Assert.ThrowsAsync<InvalidOperationException>(() => Auth.Bootstrap(app, false));
        Assert.Equal(1, await db.Users.CountAsync(user => user.Email == "admin@example.test"));
        Assert.Equal("aluno", (await db.Users.SingleAsync(user => user.Email == _email)).Role);
        builder.Environment.EnvironmentName = "Production";
        await Assert.ThrowsAsync<InvalidOperationException>(() => Auth.Bootstrap(app, true));
        await Assert.ThrowsAsync<InvalidOperationException>(() => Auth.Bootstrap(app, true, demo: true));
        await db.Users.Where(user => fixtureEmails.Contains(user.Email!) && !previousIds.Contains(user.Id)).ExecuteDeleteAsync();
    }

    [Fact]
    public async Task StaleLogoutCannotRevokeNewSession()
    {
        using var first = Client(); using var second = Client();
        await Login(first);
        await Login(second);
        Assert.Equal(HttpStatusCode.Unauthorized, (await Post(first, "/api/auth/logout", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await second.GetAsync("/api/auth/session")).StatusCode);
    }

    private sealed class FailingEmail : IRecoveryEmail
    {
        public Task<bool> CheckAvailable() => Task.FromResult(true);
        public Task Send(string email, string token) => throw new IOException("Falha fictícia de teste.");
    }
}
