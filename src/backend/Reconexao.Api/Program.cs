using Microsoft.EntityFrameworkCore;
using Npgsql;
using Reconexao.Api;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("Database");
if (string.IsNullOrWhiteSpace(connection))
    throw new InvalidOperationException("Configure a conexão Database fora do repositório.");

builder.Services.AddDbContext<FoundationDbContext>(options =>
    options.UseNpgsql(connection, postgres => postgres.SetPostgresVersion(18, 0)));
builder.Services.AddProblemDetails();
builder.AddAuth();

var app = builder.Build();
if (args.Contains("--bootstrap-admin") || args.Contains("--seed-local-auth") || args.Contains("--seed-local-demo"))
{
    await Auth.Bootstrap(app, args.Contains("--seed-local-auth") || args.Contains("--seed-local-demo"), args.Contains("--seed-local-demo"));
    return;
}
app.UseExceptionHandler();
app.Use(async (context, next) =>
{
    context.Response.Headers.CacheControl = "no-store";
    context.Response.Headers.XContentTypeOptions = "nosniff";
    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapAuth();
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" }));
app.MapGet("/health/ready", async (FoundationDbContext db, CancellationToken cancellation) =>
{
    try
    {
        var ready = await db.Markers.AsNoTracking().AnyAsync(cancellation);
        return ready
            ? Results.Ok(new { status = "ready" })
            : Results.Json(new { status = "unavailable" }, statusCode: 503);
    }
    catch (Exception exception) when (exception is NpgsqlException or TimeoutException || exception.InnerException is NpgsqlException)
    {
        return Results.Json(new { status = "unavailable" }, statusCode: 503);
    }
});
app.MapGet("/api/foundation", async (FoundationDbContext db, CancellationToken cancellation) =>
{
    try
    {
        var marker = await db.Markers.AsNoTracking().SingleAsync(cancellation);
        return Results.Ok(new { name = marker.Name, status = "ready" });
    }
    catch (Exception exception) when (exception is NpgsqlException or TimeoutException || exception.InnerException is NpgsqlException)
    {
        return Results.Problem(statusCode: 503, title: "Serviço temporariamente indisponível.");
    }
});
app.Run();

public partial class Program;
