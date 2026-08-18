using GameVault.Data.Models.Library;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<SavedGame> SavedGames => Set<SavedGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SavedGame>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.CoverUrl).HasMaxLength(1024);
            entity.HasIndex(e => e.IgdbGameId).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
