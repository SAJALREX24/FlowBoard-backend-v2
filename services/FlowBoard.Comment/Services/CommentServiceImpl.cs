using FlowBoard.Comment.Data;
using FlowBoard.Comment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Comment.Services;

public class CommentServiceImpl : ICommentService
{
    private readonly CommentDbContext _db;

    public CommentServiceImpl(CommentDbContext db)
    {
        _db = db;
    }

    public async Task<Models.Comment> CreateCommentAsync(CreateCommentRequest request)
    {
        var comment = new Models.Comment
        {
            CardId = request.CardId,
            AuthorId = request.AuthorId,
            Content = request.Content.Trim(),
            ParentCommentId = request.ParentCommentId,
            IsEdited = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return comment;
    }

    public async Task<Models.Comment?> GetCommentByIdAsync(int commentId)
    {
        var comment = await _db.Comments.FindAsync(commentId);
        if (comment == null || comment.IsDeleted)
            return null;
        return comment;
    }

    public async Task<List<Models.Comment>> GetCommentsByCardAsync(int cardId)
    {
        // Return only top-level comments (no parent), excluding soft-deleted
        return await _db.Comments
            .Where(c => c.CardId == cardId && c.ParentCommentId == null && !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Models.Comment>> GetRepliesAsync(int parentCommentId)
    {
        return await _db.Comments
            .Where(c => c.ParentCommentId == parentCommentId && !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Models.Comment?> UpdateCommentAsync(int commentId, UpdateCommentRequest request)
    {
        var comment = await _db.Comments.FindAsync(commentId);
        if (comment == null || comment.IsDeleted)
            return null;

        comment.Content = request.Content.Trim();
        comment.IsEdited = true;
        comment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        var comment = await _db.Comments.FindAsync(commentId);
        if (comment == null || comment.IsDeleted)
            return false;

        comment.IsDeleted = true;
        comment.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}