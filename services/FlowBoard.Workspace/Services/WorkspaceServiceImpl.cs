using FlowBoard.Workspace.Data;
using FlowBoard.Workspace.DTOs;
using FlowBoard.Workspace.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Workspace.Services;

public class WorkspaceServiceImpl : IWorkspaceService
{
    private readonly WorkspaceDbContext _db;

    public WorkspaceServiceImpl(WorkspaceDbContext db)
    {
        _db = db;
    }

    public async Task<Models.Workspace> CreateWorkspaceAsync(CreateWorkspaceRequest request)
    {
        var workspace = new Models.Workspace
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            OwnerId = request.OwnerId,
            Visibility = request.Visibility.ToUpperInvariant(),
            LogoUrl = request.LogoUrl?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Workspaces.Add(workspace);
        await _db.SaveChangesAsync();

        // Auto-add owner as ADMIN member
        var ownerMembership = new WorkspaceMember
        {
            WorkspaceId = workspace.WorkspaceId,
            UserId = request.OwnerId,
            Role = "ADMIN",
            JoinedAt = DateTime.UtcNow
        };

        _db.WorkspaceMembers.Add(ownerMembership);
        await _db.SaveChangesAsync();

        return workspace;
    }

    public async Task<Models.Workspace?> GetWorkspaceByIdAsync(int workspaceId)
    {
        return await _db.Workspaces.FindAsync(workspaceId);
    }

    public async Task<List<Models.Workspace>> GetWorkspacesByOwnerAsync(int ownerId)
    {
        return await _db.Workspaces
            .Where(w => w.OwnerId == ownerId && w.IsActive)
            .OrderByDescending(w => w.UpdatedAt)
            .ToListAsync();
    }

    public async Task<List<Models.Workspace>> GetWorkspacesByMemberAsync(int userId)
    {
        var workspaceIds = await _db.WorkspaceMembers
            .Where(m => m.UserId == userId)
            .Select(m => m.WorkspaceId)
            .ToListAsync();

        return await _db.Workspaces
            .Where(w => workspaceIds.Contains(w.WorkspaceId) && w.IsActive)
            .OrderByDescending(w => w.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Models.Workspace?> UpdateWorkspaceAsync(int workspaceId, UpdateWorkspaceRequest request)
    {
        var workspace = await _db.Workspaces.FindAsync(workspaceId);
        if (workspace == null || !workspace.IsActive)
            return null;

        workspace.Name = request.Name.Trim();
        workspace.Description = request.Description?.Trim();
        workspace.Visibility = request.Visibility.ToUpperInvariant();
        workspace.LogoUrl = request.LogoUrl?.Trim();
        workspace.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return workspace;
    }

    public async Task<bool> DeleteWorkspaceAsync(int workspaceId)
    {
        var workspace = await _db.Workspaces.FindAsync(workspaceId);
        if (workspace == null || !workspace.IsActive)
            return false;

        workspace.IsActive = false;
        workspace.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<WorkspaceMember> AddMemberAsync(int workspaceId, AddMemberRequest request)
    {
        var workspace = await _db.Workspaces.FindAsync(workspaceId);
        if (workspace == null || !workspace.IsActive)
            throw new InvalidOperationException("Workspace not found.");

        var existing = await _db.WorkspaceMembers
            .FirstOrDefaultAsync(m => m.WorkspaceId == workspaceId && m.UserId == request.UserId);

        if (existing != null)
            throw new InvalidOperationException("User is already a member of this workspace.");

        var member = new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = request.UserId,
            Role = request.Role.ToUpperInvariant(),
            JoinedAt = DateTime.UtcNow
        };

        _db.WorkspaceMembers.Add(member);
        await _db.SaveChangesAsync();
        return member;
    }

    public async Task<WorkspaceMember?> UpdateMemberRoleAsync(int workspaceId, int userId, UpdateMemberRoleRequest request)
    {
        var member = await _db.WorkspaceMembers
            .FirstOrDefaultAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId);

        if (member == null)
            return null;

        member.Role = request.Role;
        await _db.SaveChangesAsync();
        return member;
    }

    public async Task<bool> RemoveMemberAsync(int workspaceId, int userId)
    {
        var member = await _db.WorkspaceMembers
            .FirstOrDefaultAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId);

        if (member == null)
            return false;

        // Don't allow removing the workspace owner
        var workspace = await _db.Workspaces.FindAsync(workspaceId);
        if (workspace != null && workspace.OwnerId == userId)
            throw new InvalidOperationException("Cannot remove the workspace owner.");

        _db.WorkspaceMembers.Remove(member);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<WorkspaceMember>> GetMembersAsync(int workspaceId)
    {
        return await _db.WorkspaceMembers
            .Where(m => m.WorkspaceId == workspaceId)
            .OrderBy(m => m.JoinedAt)
            .ToListAsync();
    }
}