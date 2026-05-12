using Microsoft.EntityFrameworkCore;
using FlowBoard.Workspace.Models;

namespace FlowBoard.Workspace.Data;

public class WorkspaceDbContext : DbContext
{
    public WorkspaceDbContext(DbContextOptions<WorkspaceDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Workspace> Workspaces => Set<Models.Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Workspace>(entity =>
        {
            entity.HasIndex(w => w.OwnerId);
        });

        modelBuilder.Entity<WorkspaceMember>(entity =>
        {
            entity.HasIndex(m => new { m.WorkspaceId, m.UserId }).IsUnique();
            entity.HasIndex(m => m.UserId);
        });
    }
}