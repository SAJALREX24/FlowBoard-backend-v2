using Microsoft.EntityFrameworkCore;
using FlowBoard.Label.Models;

namespace FlowBoard.Label.Data;

public class LabelDbContext : DbContext
{
    public LabelDbContext(DbContextOptions<LabelDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Label> Labels => Set<Models.Label>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Label>(entity =>
        {
            entity.HasIndex(l => l.BoardId);
        });

        modelBuilder.Entity<ChecklistItem>(entity =>
        {
            entity.HasIndex(i => i.CardId);
            entity.HasIndex(i => new { i.CardId, i.Position });
        });
    }
}