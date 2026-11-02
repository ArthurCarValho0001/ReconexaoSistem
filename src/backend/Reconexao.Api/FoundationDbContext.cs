using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Reconexao.Api;

public sealed class FoundationDbContext(DbContextOptions<FoundationDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<FoundationMarker> Markers => Set<FoundationMarker>();
    public DbSet<PrivateReport> Reports => Set<PrivateReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.Property(user => user.Role).HasMaxLength(10).IsRequired();
            entity.HasIndex(user => user.NormalizedEmail).IsUnique();
            entity.ToTable(table => table.HasCheckConstraint("CK_User_Role", "\"Role\" IN ('admin', 'aluno')"));
        });
        modelBuilder.Entity<FoundationMarker>(entity =>
        {
            entity.ToTable("FoundationMarkers");
            entity.HasKey(marker => marker.Id);
            entity.Property(marker => marker.Name).HasMaxLength(80).IsRequired();
            entity.HasData(new FoundationMarker { Id = 1, Name = "Reconexão" });
        });
        modelBuilder.Entity<PrivateReport>(entity =>
        {
            entity.ToTable("PrivateReports", table => table.HasCheckConstraint("CK_Report_Length", "char_length(\"Text\") BETWEEN 1 AND 1000"));
            entity.HasKey(report => report.Id);
            entity.Property(report => report.Text).HasColumnType("text").IsRequired();
            entity.Property(report => report.AuthorId).IsRequired();
            entity.Property(report => report.CreatedAt).HasColumnType("timestamp with time zone");
            entity.HasOne(report => report.Author).WithMany().HasForeignKey(report => report.AuthorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(report => new { report.CreatedAt, report.Id }).IsDescending();
        });
    }
}

public sealed class FoundationMarker
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public sealed class AppUser : IdentityUser
{
    public string Role { get; set; } = "aluno";
    public string? CurrentSessionId { get; set; }
}

public sealed class PrivateReport
{
    public long Id { get; set; }
    public required string AuthorId { get; set; }
    public AppUser Author { get; set; } = null!;
    public required string Text { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
