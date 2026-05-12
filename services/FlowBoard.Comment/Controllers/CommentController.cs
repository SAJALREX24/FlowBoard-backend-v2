using FlowBoard.Comment.DTOs;
using FlowBoard.Comment.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.Comment.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
    {
        var comment = await _commentService.CreateCommentAsync(request);
        return CreatedAtAction(nameof(GetById), new { commentId = comment.CommentId }, comment);
    }

    [HttpGet("{commentId:int}")]
    public async Task<IActionResult> GetById(int commentId)
    {
        var comment = await _commentService.GetCommentByIdAsync(commentId);
        if (comment == null)
            return NotFound(new { message = "Comment not found." });

        return Ok(comment);
    }

    [HttpGet("by-card/{cardId:int}")]
    public async Task<IActionResult> GetByCard(int cardId)
    {
        var comments = await _commentService.GetCommentsByCardAsync(cardId);
        return Ok(comments);
    }

    [HttpGet("{commentId:int}/replies")]
    public async Task<IActionResult> GetReplies(int commentId)
    {
        var replies = await _commentService.GetRepliesAsync(commentId);
        return Ok(replies);
    }

    [HttpPut("{commentId:int}")]
    public async Task<IActionResult> Update(int commentId, [FromBody] UpdateCommentRequest request)
    {
        var updated = await _commentService.UpdateCommentAsync(commentId, request);
        if (updated == null)
            return NotFound(new { message = "Comment not found." });

        return Ok(updated);
    }

    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> Delete(int commentId)
    {
        var deleted = await _commentService.DeleteCommentAsync(commentId);
        if (!deleted)
            return NotFound(new { message = "Comment not found or already deleted." });

        return NoContent();
    }
}