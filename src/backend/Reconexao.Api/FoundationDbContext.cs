using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Reconexao.Api;

public sealed class FoundationDbContext(DbContextOptions<FoundationDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<FoundationMarker> Markers => Set<FoundationMarker>();

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
