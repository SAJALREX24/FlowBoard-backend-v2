using FlowBoard.Board.DTOs;
using FlowBoard.Board.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.Board.Controllers;

[ApiController]
[Route("api/boards")]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;

    public BoardController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBoardRequest request)
    {
        var board = await _boardService.CreateBoardAsync(request);
        return CreatedAtAction(nameof(GetById), new { boardId = board.BoardId }, board);
    }

    [HttpGet("{boardId:int}")]
    public async Task<IActionResult> GetById(int boardId)
    {
        var board = await _boardService.GetBoardByIdAsync(boardId);
        if (board == null)
            return NotFound(new { message = "Board not found." });

        return Ok(board);
    }

    [HttpGet("by-workspace/{workspaceId:int}")]
    public async Task<IActionResult> GetByWorkspace(int workspaceId)
    {
        var boards = await _boardService.GetBoardsByWorkspaceAsync(workspaceId);
        return Ok(boards);
    }

    [HttpGet("by-member/{userId:int}")]
    public async Task<IActionResult> GetByMember(int userId)
    {
        var boards = await _boardService.GetBoardsByMemberAsync(userId);
        return Ok(boards);
    }

    [HttpPut("{boardId:int}")]
    public async Task<IActionResult> Update(int boardId, [FromBody] UpdateBoardRequest request)
    {
        var updated = await _boardService.UpdateBoardAsync(boardId, request);
        if (updated == null)
            return NotFound(new { message = "Board not found." });

        return Ok(updated);
    }

    [HttpPost("{boardId:int}/close")]
    public async Task<IActionResult> Close(int boardId)
    {
        var closed = await _boardService.CloseBoardAsync(boardId);
        if (!closed)
            return NotFound(new { message = "Board not found or already closed." });

        return NoContent();
    }

    [HttpPost("{boardId:int}/reopen")]
    public async Task<IActionResult> Reopen(int boardId)
    {
        var reopened = await _boardService.ReopenBoardAsync(boardId);
        if (!reopened)
            return NotFound(new { message = "Board not found or not closed." });

        return NoContent();
    }

    [HttpPost("{boardId:int}/members")]
    public async Task<IActionResult> AddMember(int boardId, [FromBody] AddBoardMemberRequest request)
    {
        try
        {
            var member = await _boardService.AddMemberAsync(boardId, request);
            return Ok(member);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{boardId:int}/members/{userId:int}")]
    public async Task<IActionResult> UpdateMemberRole(int boardId, int userId, [FromBody] UpdateBoardMemberRoleRequest request)
    {
        var updated = await _boardService.UpdateMemberRoleAsync(boardId, userId, request);
        if (updated == null)
            return NotFound(new { message = "Member not found." });

        return Ok(updated);
    }

    [HttpDelete("{boardId:int}/members/{userId:int}")]
    public async Task<IActionResult> RemoveMember(int boardId, int userId)
    {
        try
        {
            var removed = await _boardService.RemoveMemberAsync(boardId, userId);
            if (!removed)
                return NotFound(new { message = "Member not found." });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{boardId:int}/members")]
    public async Task<IActionResult> GetMembers(int boardId)
    {
        var members = await _boardService.GetMembersAsync(boardId);
        return Ok(members);
    }
}