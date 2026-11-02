using System.Buffers;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Reconexao.Api;

public static class Reports
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static void MapReports(this WebApplication app)
    {
        app.MapPost("/api/relatos", Submit).RequireAuthorization(policy => policy.RequireRole("aluno"));
        app.MapGet("/api/admin/relatos", List).RequireAuthorization(policy => policy.RequireRole("admin"));
    }

    public static bool TryNormalize(string? input, out string text)
    {
        text = input?.Replace("\r\n", "\n").Replace('\r', '\n') ?? "";
        var remaining = text.AsSpan();
        var count = 0;
        var hasContent = false;
        while (!remaining.IsEmpty)
        {
            if (Rune.DecodeFromUtf16(remaining, out var rune, out var consumed) != OperationStatus.Done || rune.Value == 0)
                return false;
            if (++count > 1000) return false;
            hasContent |= !Rune.IsWhiteSpace(rune);
            remaining = remaining[consumed..];
        }
        return count > 0 && hasContent;
    }

    private static async Task<IResult> Submit(HttpContext context, FoundationDbContext db, IAntiforgery csrf, CancellationToken cancellation)
    {
        try { await csrf.ValidateRequestAsync(context); }
        catch (AntiforgeryValidationException) { return InvalidRequest(); }
        if (!context.Request.HasJsonContentType()) return InvalidRequest();
        ReportInput? input;
        try
        {
            var buffer = new byte[16_385];
            var length = 0;
            while (length < buffer.Length)
            {
                var read = await context.Request.Body.ReadAsync(buffer.AsMemory(length), cancellation);
                if (read == 0) break;
                length += read;
            }
            if (length > 16_384) return InvalidRequest();
            input = JsonSerializer.Deserialize<ReportInput>(buffer.AsSpan(0, length), Json);
        }
        catch (JsonException) { return InvalidRequest(); }
        if (!TryNormalize(input?.Text, out var text)) return InvalidRequest();
        var author = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (author is null) return Results.Unauthorized();
        db.Reports.Add(new PrivateReport { AuthorId = author, Text = text, CreatedAt = DateTimeOffset.UtcNow });
        try { await db.SaveChangesAsync(cancellation); }
        catch (Exception exception) when (IsDatabaseFailure(exception)) { return Unavailable(); }
        return Results.Json(new { status = "received" }, statusCode: StatusCodes.Status201Created);
    }

    private static async Task<IResult> List(HttpContext context, FoundationDbContext db, CancellationToken cancellation)
    {
        var limit = 20;
        if (context.Request.Query.TryGetValue("limit", out var values) &&
            (values.Count != 1 || !int.TryParse(values[0], NumberStyles.None, CultureInfo.InvariantCulture, out limit) || limit is < 1 or > 50))
            return InvalidRequest();
        ReportCursor? cursor = null;
        if (context.Request.Query.TryGetValue("cursor", out var cursors))
        {
            if (cursors.Count != 1 || string.IsNullOrEmpty(cursors[0]) || cursors[0]!.Length > 512) return InvalidRequest();
            try { cursor = JsonSerializer.Deserialize<ReportCursor>(WebEncoders.Base64UrlDecode(cursors[0]!), Json); }
            catch (Exception exception) when (exception is FormatException or JsonException or ArgumentException) { return InvalidRequest(); }
            if (cursor is null || cursor.CreatedAt.Offset != TimeSpan.Zero ||
                !long.TryParse(cursor.Id, NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0) return InvalidRequest();
        }
        var query = db.Reports.AsNoTracking();
        if (cursor is not null)
        {
            var id = long.Parse(cursor.Id, CultureInfo.InvariantCulture);
            var instant = cursor.CreatedAt;
            query = query.Where(report => report.CreatedAt < instant || (report.CreatedAt == instant && report.Id < id));
        }
        List<ReportRow> rows;
        try
        {
            rows = await query.OrderByDescending(report => report.CreatedAt).ThenByDescending(report => report.Id)
                .Take(limit + 1).Select(report => new ReportRow(report.Id, report.AuthorId, report.Author.Email, report.CreatedAt, report.Text))
                .ToListAsync(cancellation);
        }
        catch (Exception exception) when (IsDatabaseFailure(exception)) { return Unavailable(); }
        var hasMore = rows.Count > limit;
        var page = rows.Take(limit).ToList();
        var last = page.LastOrDefault();
        var nextCursor = hasMore && last is not null
            ? WebEncoders.Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new ReportCursor(last.CreatedAt, last.Id.ToString(CultureInfo.InvariantCulture)), Json))
            : null;
        return Results.Ok(new
        {
            items = page.Select(row => new { id = row.Id.ToString(CultureInfo.InvariantCulture), author = new { id = row.AuthorId, email = row.Email }, row.CreatedAt, row.Text }),
            nextCursor
        });
    }

    private static bool IsDatabaseFailure(Exception exception) => exception is NpgsqlException or TimeoutException or DbUpdateException || exception.InnerException is NpgsqlException;
    private static IResult InvalidRequest() => Results.Problem(statusCode: 400, title: "Não foi possível validar o pedido de relato.");
    private static IResult Unavailable() => Results.Problem(statusCode: 503, title: "Relatos temporariamente indisponíveis.");

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    private sealed record ReportInput(string? Text);
    private sealed record ReportCursor(DateTimeOffset CreatedAt, string Id);
    private sealed record ReportRow(long Id, string AuthorId, string? Email, DateTimeOffset CreatedAt, string Text);
}
