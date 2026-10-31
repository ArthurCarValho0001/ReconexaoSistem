using Microsoft.EntityFrameworkCore;

namespace Reconexao.Api;

public sealed class FoundationDbContext(DbContextOptions<FoundationDbContext> options) : DbContext(options)
{
    public DbSet<FoundationMarker> Markers => Set<FoundationMarker>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
