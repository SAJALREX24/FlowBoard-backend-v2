using FlowBoard.Workspace.DTOs;
using FlowBoard.Workspace.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.Workspace.Controllers;

[ApiController]
[Route("api/workspaces")]
public class WorkspaceController : ControllerBase
{
    private readonly IWorkspaceService _workspaceService;

    public WorkspaceController(IWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkspaceRequest request)
    {
        var workspace = await _workspaceService.CreateWorkspaceAsync(request);
        return CreatedAtAction(nameof(GetById), new { workspaceId = workspace.WorkspaceId }, workspace);
    }

    [HttpGet("{workspaceId:int}")]
    public async Task<IActionResult> GetById(int workspaceId)
    {
        var workspace = await _workspaceService.GetWorkspaceByIdAsync(workspaceId);
        if (workspace == null || !workspace.IsActive)
            return NotFound(new { message = "Workspace not found." });

        return Ok(workspace);
    }

    [HttpGet("by-owner/{ownerId:int}")]
    public async Task<IActionResult> GetByOwner(int ownerId)
    {
        var workspaces = await _workspaceService.GetWorkspacesByOwnerAsync(ownerId);
        return Ok(workspaces);
    }

    [HttpGet("by-member/{userId:int}")]
    public async Task<IActionResult> GetByMember(int userId)
    {
        var workspaces = await _workspaceService.GetWorkspacesByMemberAsync(userId);
        return Ok(workspaces);
    }

    [HttpPut("{workspaceId:int}")]
    public async Task<IActionResult> Update(int workspaceId, [FromBody] UpdateWorkspaceRequest request)
    {
        var updated = await _workspaceService.UpdateWorkspaceAsync(workspaceId, request);
        if (updated == null)
            return NotFound(new { message = "Workspace not found." });

        return Ok(updated);
    }

    [HttpDelete("{workspaceId:int}")]
    public async Task<IActionResult> Delete(int workspaceId)
    {
        var deleted = await _workspaceService.DeleteWorkspaceAsync(workspaceId);
        if (!deleted)
            return NotFound(new { message = "Workspace not found." });

        return NoContent();
    }

    [HttpPost("{workspaceId:int}/members")]
    public async Task<IActionResult> AddMember(int workspaceId, [FromBody] AddMemberRequest request)
    {
        try
        {
            var member = await _workspaceService.AddMemberAsync(workspaceId, request);
            return Ok(member);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{workspaceId:int}/members/{userId:int}")]
    public async Task<IActionResult> UpdateMemberRole(int workspaceId, int userId, [FromBody] UpdateMemberRoleRequest request)
    {
        var updated = await _workspaceService.UpdateMemberRoleAsync(workspaceId, userId, request);
        if (updated == null)
            return NotFound(new { message = "Member not found." });

        return Ok(updated);
    }

    [HttpDelete("{workspaceId:int}/members/{userId:int}")]
    public async Task<IActionResult> RemoveMember(int workspaceId, int userId)
    {
        try
        {
            var removed = await _workspaceService.RemoveMemberAsync(workspaceId, userId);
            if (!removed)
                return NotFound(new { message = "Member not found." });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{workspaceId:int}/members")]
    public async Task<IActionResult> GetMembers(int workspaceId)
    {
        var members = await _workspaceService.GetMembersAsync(workspaceId);
        return Ok(members);
    }
}
