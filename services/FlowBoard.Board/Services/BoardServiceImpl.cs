using FlowBoard.Board.Data;
using FlowBoard.Board.DTOs;
using FlowBoard.Board.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Board.Services;

public class BoardServiceImpl : IBoardService
{
    private readonly BoardDbContext _db;

    public BoardServiceImpl(BoardDbContext db)
    {
        _db = db;
    }

    public async Task<Models.Board> CreateBoardAsync(CreateBoardRequest request)
    {
        var board = new Models.Board
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            WorkspaceId = request.WorkspaceId,
            CreatedBy = request.CreatedBy,
            Visibility = request.Visibility.ToUpperInvariant(),
            BackgroundColor = request.BackgroundColor?.Trim(),
            IsClosed = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Boards.Add(board);
        await _db.SaveChangesAsync();

        // Auto-add creator as ADMIN member
        var adminMembership = new BoardMember
        {
            BoardId = board.BoardId,
            UserId = request.CreatedBy,
            Role = "ADMIN",
            JoinedAt = DateTime.UtcNow
        };

        _db.BoardMembers.Add(adminMembership);
        await _db.SaveChangesAsync();

        return board;
    }

    public async Task<Models.Board?> GetBoardByIdAsync(int boardId)
    {
        return await _db.Boards.FindAsync(boardId);
    }

    public async Task<List<Models.Board>> GetBoardsByWorkspaceAsync(int workspaceId)
    {
        return await _db.Boards
            .Where(b => b.WorkspaceId == workspaceId && !b.IsClosed)
            .OrderByDescending(b => b.UpdatedAt)
            .ToListAsync();
    }

    public async Task<List<Models.Board>> GetBoardsByMemberAsync(int userId)
    {
        var boardIds = await _db.BoardMembers
            .Where(m => m.UserId == userId)
            .Select(m => m.BoardId)
            .ToListAsync();

        return await _db.Boards
            .Where(b => boardIds.Contains(b.BoardId) && !b.IsClosed)
            .OrderByDescending(b => b.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Models.Board?> UpdateBoardAsync(int boardId, UpdateBoardRequest request)
    {
        var board = await _db.Boards.FindAsync(boardId);
        if (board == null)
            return null;

        board.Name = request.Name.Trim();
        board.Description = request.Description?.Trim();
        board.Visibility = request.Visibility.ToUpperInvariant();
        board.BackgroundColor = request.BackgroundColor?.Trim();
        board.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return board;
    }

    public async Task<bool> CloseBoardAsync(int boardId)
    {
        var board = await _db.Boards.FindAsync(boardId);
        if (board == null || board.IsClosed)
            return false;

        board.IsClosed = true;
        board.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReopenBoardAsync(int boardId)
    {
        var board = await _db.Boards.FindAsync(boardId);
        if (board == null || !board.IsClosed)
            return false;

        board.IsClosed = false;
        board.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<BoardMember> AddMemberAsync(int boardId, AddBoardMemberRequest request)
    {
        var board = await _db.Boards.FindAsync(boardId);
        if (board == null || board.IsClosed)
            throw new InvalidOperationException("Board not found or is closed.");

        var existing = await _db.BoardMembers
            .FirstOrDefaultAsync(m => m.BoardId == boardId && m.UserId == request.UserId);

        if (existing != null)
            throw new InvalidOperationException("User is already a member of this board.");

        var member = new BoardMember
        {
            BoardId = boardId,
            UserId = request.UserId,
            Role = request.Role.ToUpperInvariant(),
            JoinedAt = DateTime.UtcNow
        };

        _db.BoardMembers.Add(member);
        await _db.SaveChangesAsync();
        return member;
    }

    public async Task<BoardMember?> UpdateMemberRoleAsync(int boardId, int userId, UpdateBoardMemberRoleRequest request)
    {
        var member = await _db.BoardMembers
            .FirstOrDefaultAsync(m => m.BoardId == boardId && m.UserId == userId);

        if (member == null)
            return null;

        member.Role = request.Role;
        await _db.SaveChangesAsync();
        return member;
    }

    public async Task<bool> RemoveMemberAsync(int boardId, int userId)
    {
        var member = await _db.BoardMembers
            .FirstOrDefaultAsync(m => m.BoardId == boardId && m.UserId == userId);

        if (member == null)
            return false;

        // Don't allow removing the board creator
        var board = await _db.Boards.FindAsync(boardId);
        if (board != null && board.CreatedBy == userId)
            throw new InvalidOperationException("Cannot remove the board creator.");

        _db.BoardMembers.Remove(member);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<BoardMember>> GetMembersAsync(int boardId)
    {
        return await _db.BoardMembers
            .Where(m => m.BoardId == boardId)
            .OrderBy(m => m.JoinedAt)
            .ToListAsync();
    }
}