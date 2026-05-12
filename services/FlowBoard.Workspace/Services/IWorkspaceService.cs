using FlowBoard.Workspace.DTOs;
using FlowBoard.Workspace.Models;

namespace FlowBoard.Workspace.Services;

public interface IWorkspaceService
{
    Task<Models.Workspace> CreateWorkspaceAsync(CreateWorkspaceRequest request);
    Task<Models.Workspace?> GetWorkspaceByIdAsync(int workspaceId);
    Task<List<Models.Workspace>> GetWorkspacesByOwnerAsync(int ownerId);
    Task<List<Models.Workspace>> GetWorkspacesByMemberAsync(int userId);
    Task<Models.Workspace?> UpdateWorkspaceAsync(int workspaceId, UpdateWorkspaceRequest request);
    Task<bool> DeleteWorkspaceAsync(int workspaceId);
    Task<WorkspaceMember> AddMemberAsync(int workspaceId, AddMemberRequest request);
    Task<WorkspaceMember?> UpdateMemberRoleAsync(int workspaceId, int userId, UpdateMemberRoleRequest request);
    Task<bool> RemoveMemberAsync(int workspaceId, int userId);
    Task<List<WorkspaceMember>> GetMembersAsync(int workspaceId);
}