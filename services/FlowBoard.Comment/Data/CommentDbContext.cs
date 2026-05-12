using Microsoft.EntityFrameworkCore;
using FlowBoard.Comment.Models;

namespace FlowBoard.Comment.Data;

public class CommentDbContext : DbContext
{
    public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Comment> Comments => Set<Models.Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Comment>(entity =>
        {
            entity.HasIndex(c => c.CardId);
            entity.HasIndex(c => c.AuthorId);
            entity.HasIndex(c => c.ParentCommentId);
        });
    }
}