using FlowBoard.Comment.DTOs;
using FlowBoard.Comment.Models;

namespace FlowBoard.Comment.Services;

public interface ICommentService
{
    Task<Models.Comment> CreateCommentAsync(CreateCommentRequest request);
    Task<Models.Comment?> GetCommentByIdAsync(int commentId);
    Task<List<Models.Comment>> GetCommentsByCardAsync(int cardId);
    Task<List<Models.Comment>> GetRepliesAsync(int parentCommentId);
    Task<Models.Comment?> UpdateCommentAsync(int commentId, UpdateCommentRequest request);
    Task<bool> DeleteCommentAsync(int commentId);
}