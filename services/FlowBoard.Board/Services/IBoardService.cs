using FlowBoard.Board.DTOs;
using FlowBoard.Board.Models;

namespace FlowBoard.Board.Services;

public interface IBoardService
{
    Task<Models.Board> CreateBoardAsync(CreateBoardRequest request);
    Task<Models.Board?> GetBoardByIdAsync(int boardId);
    Task<List<Models.Board>> GetBoardsByWorkspaceAsync(int workspaceId);
    Task<List<Models.Board>> GetBoardsByMemberAsync(int userId);
    Task<Models.Board?> UpdateBoardAsync(int boardId, UpdateBoardRequest request);
    Task<bool> CloseBoardAsync(int boardId);
    Task<bool> ReopenBoardAsync(int boardId);
    Task<BoardMember> AddMemberAsync(int boardId, AddBoardMemberRequest request);
    Task<BoardMember?> UpdateMemberRoleAsync(int boardId, int userId, UpdateBoardMemberRoleRequest request);
    Task<bool> RemoveMemberAsync(int boardId, int userId);
    Task<List<BoardMember>> GetMembersAsync(int boardId);
}