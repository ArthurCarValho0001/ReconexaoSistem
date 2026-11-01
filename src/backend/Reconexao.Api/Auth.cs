using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Reconexao.Api;

public static class Auth
{
    public const string Scheme = "Reconexao";
    public static bool IsLocal(IHostEnvironment environment) => environment.IsDevelopment() || environment.IsEnvironment("Test");

    public static void AddAuth(this WebApplicationBuilder builder)
    {
        var keys = builder.Configuration["Auth:KeysPath"];
        var protection = builder.Services.AddDataProtection().SetApplicationName("Reconexao");
        if (!string.IsNullOrWhiteSpace(keys)) protection.PersistKeysToFileSystem(new DirectoryInfo(keys));
        else if (!IsLocal(builder.Environment)) throw new InvalidOperationException("Configure Auth:KeysPath fora do repositório.");
        if (!IsLocal(builder.Environment) && builder.Configuration["Auth:EmailAdapter"] == "Local")
            throw new InvalidOperationException("Adapter de email local proibido neste ambiente.");
        builder.Services.AddIdentityCore<AppUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 12;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        }).AddEntityFrameworkStores<FoundationDbContext>().AddDefaultTokenProviders();
        builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(30));
        builder.Services.AddAuthentication(Scheme).AddCookie(Scheme, options =>
        {
            options.Cookie.Name = "__Host-Reconexao";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = false;
            options.Events.OnRedirectToLogin = context => { context.Response.StatusCode = 401; return Task.CompletedTask; };
            options.Events.OnRedirectToAccessDenied = context => { context.Response.StatusCode = 403; return Task.CompletedTask; };
            options.Events.OnValidatePrincipal = async context =>
            {
                var db = context.HttpContext.RequestServices.GetRequiredService<FoundationDbContext>();
                var id = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                var session = context.Principal?.FindFirstValue("session");
                var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == id);
                if (user is null || user.CurrentSessionId is null || user.CurrentSessionId != session ||
                    user.Role != context.Principal?.FindFirstValue(ClaimTypes.Role) ||
                    user.SecurityStamp != context.Principal?.FindFirstValue("stamp")) context.RejectPrincipal();
            };
        });
        builder.Services.AddAuthorization();
        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "__Host-Reconexao-Csrf";
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = 429;
            options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "local",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
        });
        builder.Services.AddSingleton<IRecoveryEmail, RecoveryEmail>();
    }

    public static void MapAuth(this WebApplication app)
    {
        app.MapGet("/api/auth/csrf", (HttpContext context, IAntiforgery csrf) =>
        {
            context.Response.Headers.CacheControl = "no-cache, no-store";
            context.Response.Headers.Pragma = "no-cache";
            return Results.Ok(new { token = csrf.GetAndStoreTokens(context).RequestToken });
        });
        var group = app.MapGroup("/api/auth").AddEndpointFilter(async (context, next) =>
        {
            if (HttpMethods.IsPost(context.HttpContext.Request.Method))
            {
                try { await context.HttpContext.RequestServices.GetRequiredService<IAntiforgery>().ValidateRequestAsync(context.HttpContext); }
                catch (AntiforgeryValidationException) { return Results.Problem(statusCode: 400, title: "Requisição inválida. Atualize a página."); }
            }
            return await next(context);
        });
        group.MapPost("/login", Login).RequireRateLimiting("auth");
        group.MapGet("/session", (ClaimsPrincipal principal) => Results.Ok(Session(principal))).RequireAuthorization();
        group.MapPost("/logout", Logout).RequireAuthorization();
        group.MapPost("/forgot-password", Forgot).RequireRateLimiting("auth");
        group.MapPost("/reset-password", Reset).RequireRateLimiting("auth");
        app.MapGet("/api/admin/session", (ClaimsPrincipal principal) => Results.Ok(Session(principal)))
            .RequireAuthorization(policy => policy.RequireRole("admin"));
    }

    private static object Session(ClaimsPrincipal principal) => new
    {
        id = principal.FindFirstValue(ClaimTypes.NameIdentifier), email = principal.FindFirstValue(ClaimTypes.Email),
        role = principal.FindFirstValue(ClaimTypes.Role)
    };

    private static async Task<AppUser?> LockUser(FoundationDbContext db, UserManager<AppUser> manager, string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 254) return null;
        var normalized = manager.NormalizeEmail(email.Trim());
        return await db.Users.FromSqlInterpolated($"SELECT * FROM \"AspNetUsers\" WHERE \"NormalizedEmail\" = {normalized} FOR UPDATE").SingleOrDefaultAsync();
    }

    private static async Task<IResult> Login(LoginInput input, HttpContext context, FoundationDbContext db, UserManager<AppUser> manager)
    {
        if (string.IsNullOrEmpty(input.Password) || input.Password.Length > 256) return InvalidLogin();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var user = await LockUser(db, manager, input.Email);
        if (user is null)
        {
            _ = new PasswordHasher<AppUser>().VerifyHashedPassword(new AppUser(), DummyHash, input.Password);
            return InvalidLogin();
        }
        if (await manager.IsLockedOutAsync(user)) return InvalidLogin();
        if (!await manager.CheckPasswordAsync(user, input.Password))
        {
            await manager.AccessFailedAsync(user);
            await transaction.CommitAsync();
            return InvalidLogin();
        }
        await manager.ResetAccessFailedCountAsync(user);
        user.CurrentSessionId = Guid.NewGuid().ToString("N");
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, user.Role), new Claim("session", user.CurrentSessionId),
            new Claim("stamp", user.SecurityStamp!)
        }, Scheme));
        await context.SignInAsync(Scheme, principal);
        return Results.Ok(Session(principal));
    }

    private static readonly string DummyHash = new PasswordHasher<AppUser>().HashPassword(new AppUser(), Guid.NewGuid().ToString("N"));
    private static IResult InvalidLogin() => Results.Problem(statusCode: 401, title: "Email ou senha inválidos, ou acesso temporariamente bloqueado.");

    private static async Task<IResult> Logout(HttpContext context, FoundationDbContext db, UserManager<AppUser> manager)
    {
        var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var session = context.User.FindFirstValue("session");
        await using var transaction = await db.Database.BeginTransactionAsync();
        var address = context.User.FindFirstValue(ClaimTypes.Email);
        var user = await LockUser(db, manager, address);
        if (user is not null && user.Id == id && user.CurrentSessionId == session)
        {
            user.CurrentSessionId = null;
            await db.SaveChangesAsync();
        }
        await transaction.CommitAsync();
        await context.SignOutAsync(Scheme);
        return Results.NoContent();
    }

    private static async Task<IResult> Forgot(ForgotInput input, UserManager<AppUser> manager, IRecoveryEmail email, ILogger<RecoveryEmail> logger)
    {
        if (!await email.CheckAvailable()) return Results.Problem(statusCode: 503, title: "Recuperação temporariamente indisponível.");
        if (string.IsNullOrWhiteSpace(input.Email) || input.Email.Length > 254) return Results.Accepted();
        var user = await manager.FindByEmailAsync(input.Email.Trim());
        if (user is not null)
        {
            var token = await manager.GeneratePasswordResetTokenAsync(user);
            try { await email.Send(user.Email!, token); }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                logger.LogWarning("Falha de entrega no adapter local de recuperação; pedido não comprova entrega.");
            }
        }
        return Results.Accepted();
    }

    private static async Task<IResult> Reset(ResetInput input, FoundationDbContext db, UserManager<AppUser> manager)
    {
        if (string.IsNullOrEmpty(input.Token) || input.Token.Length > 4096 || string.IsNullOrEmpty(input.Password) || input.Password.Length > 256)
            return InvalidReset();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var user = await LockUser(db, manager, input.Email);
        if (user is null) return InvalidReset();
        var result = await manager.ResetPasswordAsync(user, input.Token, input.Password);
        if (!result.Succeeded) return InvalidReset();
        user.CurrentSessionId = null;
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return Results.NoContent();
    }

    private static IResult InvalidReset() => Results.Problem(statusCode: 400, title: "Não foi possível redefinir. Confira token e senha (mínimo 12 caracteres, maiúscula, minúscula, número e símbolo).");

    public static async Task Bootstrap(WebApplication app, bool seed, bool demo = false)
    {
        if (seed && !IsLocal(app.Environment)) throw new InvalidOperationException("Contas fictícias somente em Development/Test.");
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoundationDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var password = app.Configuration[seed ? "LocalAuth:Password" : "Bootstrap:Password"];
        var email = app.Configuration["Bootstrap:Email"];
        if (string.IsNullOrWhiteSpace(password) || (!seed && string.IsNullOrWhiteSpace(email)))
            throw new InvalidOperationException("Configure credenciais de bootstrap no ambiente, nunca nos argumentos.");
        await using var transaction = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(72502002)");
        if (seed)
        {
            var accounts = demo
                ? new[] { ("aluno.a@example.test", "aluno"), ("admin@example.test", "admin") }
                : new[] { ("aluno.a@example.test", "aluno"), ("aluno.b@example.test", "aluno"), ("admin@example.test", "admin") };
            foreach (var item in accounts)
                await Create(item.Item1, item.Item2);
        }
        else
        {
            if (await db.Users.AnyAsync(user => user.Role == "admin")) throw new InvalidOperationException("Bootstrap já realizado; administrador existente.");
            await Create(email!, "admin");
        }
        await transaction.CommitAsync();
        async Task Create(string address, string role)
        {
            var existing = await users.FindByEmailAsync(address);
            if (existing is not null)
            {
                if (!seed || existing.Role != role) throw new InvalidOperationException("Conta já existente; nenhuma promoção automática.");
                return;
            }
            var result = await users.CreateAsync(new AppUser { UserName = address, Email = address, Role = role }, password);
            if (!result.Succeeded) throw new InvalidOperationException("Bootstrap recusado; confira email e política de senha.");
        }
    }
    public sealed record LoginInput(string? Email, string? Password);
    public sealed record ForgotInput(string? Email);
    public sealed record ResetInput(string? Email, string? Token, string? Password);
}

public interface IRecoveryEmail
{
    Task<bool> CheckAvailable();
    Task Send(string email, string token);
}

public sealed class RecoveryEmail(IConfiguration configuration, IHostEnvironment environment) : IRecoveryEmail
{
    public bool Available => Auth.IsLocal(environment) && configuration["Auth:EmailAdapter"] == "Local" &&
        !string.IsNullOrWhiteSpace(configuration["Auth:LocalEmailPath"]);
    public async Task<bool> CheckAvailable()
    {
        if (!Available) return false;
        try
        {
            var directory = configuration["Auth:LocalEmailPath"]!;
            Directory.CreateDirectory(directory);
            var probe = Path.Combine(directory, $"{Guid.NewGuid():N}.probe");
            await File.WriteAllTextAsync(probe, "");
            File.Delete(probe);
            return true;
        }
        catch (IOException) { return false; }
        catch (UnauthorizedAccessException) { return false; }
    }
    public async Task Send(string email, string token)
    {
        if (!Available) throw new InvalidOperationException("Email local indisponível.");
        var directory = configuration["Auth:LocalEmailPath"]!;
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, $"{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(new { adapter = "LOCAL_SIMULADO", email, token }));
    }
}
