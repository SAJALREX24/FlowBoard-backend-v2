using Microsoft.EntityFrameworkCore;
using FlowBoard.Card.Models;

namespace FlowBoard.Card.Data;

public class CardDbContext : DbContext
{
    public CardDbContext(DbContextOptions<CardDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Card> Cards => Set<Models.Card>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Card>(entity =>
        {
            entity.HasIndex(c => c.ListId);
            entity.HasIndex(c => c.BoardId);
            entity.HasIndex(c => new { c.ListId, c.Position });
            entity.HasIndex(c => c.AssigneeId);
            entity.HasIndex(c => c.DueDate);
        });
    }
}