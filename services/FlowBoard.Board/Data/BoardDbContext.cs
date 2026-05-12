using Microsoft.EntityFrameworkCore;
using FlowBoard.Board.Models;

namespace FlowBoard.Board.Data;

public class BoardDbContext : DbContext
{
    public BoardDbContext(DbContextOptions<BoardDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Board> Boards => Set<Models.Board>();
    public DbSet<BoardMember> BoardMembers => Set<BoardMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Board>(entity =>
        {
            entity.HasIndex(b => b.WorkspaceId);
            entity.HasIndex(b => b.CreatedBy);
        });

        modelBuilder.Entity<BoardMember>(entity =>
        {
            entity.HasIndex(m => new { m.BoardId, m.UserId }).IsUnique();
            entity.HasIndex(m => m.UserId);
        });
    }
}