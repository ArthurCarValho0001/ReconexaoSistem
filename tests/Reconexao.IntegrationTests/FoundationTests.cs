using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Reconexao.Api;
using Xunit;

namespace Reconexao.IntegrationTests;

public sealed class FoundationTests : IAsyncLifetime
{
    private string _connection = "";

    public async Task InitializeAsync()
    {
        _connection = Environment.GetEnvironmentVariable("RECONEXAO_TEST_CONNECTION")
            ?? throw new InvalidOperationException("Use node scripts/local.mjs test com o banco descartável iniciado.");
        RequireTestDatabase(_connection);
        await using var db = CreateDb(_connection);
        await db.Database.MigrateAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("Host=127.0.0.1;Port=55432;Database=reconexao_dev;Username=reconexao_dev_app")]
    [InlineData("Host=127.0.0.1;Port=55433;Database=reconexao_dev;Username=reconexao_test_app")]
    [InlineData("Host=remote;Port=55433;Database=reconexao_test;Username=reconexao_test_app")]
    public void RejectsNonTestDatabase(string connection) =>
        Assert.Throws<InvalidOperationException>(() => RequireTestDatabase(connection));

    [Fact]
    public async Task MigrationAndRollbackWorkOnPostgresWithoutSuperuser()
    {
        await using var db = CreateDb(_connection);
        Assert.Equal("Reconexão", (await db.Markers.SingleAsync()).Name);
        await using var transaction = await db.Database.BeginTransactionAsync();
        db.Markers.Add(new FoundationMarker { Id = 2, Name = "Somente teste fictício" });
        await db.SaveChangesAsync();
        Assert.Equal(2, await db.Markers.CountAsync());
        await transaction.RollbackAsync();
        Assert.Equal(1, await db.Markers.AsNoTracking().CountAsync());
        await using var connection = new NpgsqlConnection(_connection);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("SELECT rolsuper FROM pg_roles WHERE rolname = current_user", connection);
        Assert.Equal(false, await command.ExecuteScalarAsync());
    }

    [Theory]
    [InlineData("/health/live", "alive")]
    [InlineData("/health/ready", "ready")]
    [InlineData("/api/foundation", "Reconexão")]
    public async Task ApiUsesRealDatabase(string route, string expected)
    {
        await using var factory = new ApiFactory(_connection);
        using var client = factory.CreateClient();
        var response = await client.GetAsync(route);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(expected, System.Text.RegularExpressions.Regex.Unescape(await response.Content.ReadAsStringAsync()));
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }

    [Theory]
    [InlineData("/health/ready")]
    [InlineData("/api/foundation")]
    public async Task DatabaseFailureReturnsUnavailableWithoutConnectionDetails(string route)
    {
        var unavailable = new NpgsqlConnectionStringBuilder(_connection) { Port = 55434, Timeout = 1 };
        await using var factory = new ApiFactory(unavailable.ConnectionString);
        using var client = factory.CreateClient();
        var response = await client.GetAsync(route);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("Password", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("55434", body);
        Assert.DoesNotContain("Npgsql", body);
    }

    private static void RequireTestDatabase(string connection)
    {
        var settings = new NpgsqlConnectionStringBuilder(connection);
        if (settings.Host != "127.0.0.1" || settings.Port != 55433 ||
            settings.Database != "reconexao_test" || settings.Username != "reconexao_test_app")
            throw new InvalidOperationException("Testes permitidos somente no banco descartável isolado.");
    }

    private static FoundationDbContext CreateDb(string connection) => new(
        new DbContextOptionsBuilder<FoundationDbContext>().UseNpgsql(connection,
            options => options.SetPostgresVersion(18, 0)).Options);

    private sealed class ApiFactory(string connection) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.UseSetting("ConnectionStrings:Database", connection);
        }
    }
}
