using Microsoft.EntityFrameworkCore;
using FlowBoard.List.Models;

namespace FlowBoard.List.Data;

public class ListDbContext : DbContext
{
    public ListDbContext(DbContextOptions<ListDbContext> options) : base(options)
    {
    }

    public DbSet<Models.List> Lists => Set<Models.List>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.List>(entity =>
        {
            entity.HasIndex(l => l.BoardId);
            entity.HasIndex(l => new { l.BoardId, l.Position });
        });
    }
}