using FlowBoard.List.Data;
using FlowBoard.List.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.List.Services;

public class ListServiceImpl : IListService
{
    private readonly ListDbContext _db;

    public ListServiceImpl(ListDbContext db)
    {
        _db = db;
    }

    public async Task<Models.List> CreateListAsync(CreateListRequest request)
    {
        // Auto-position at end: maxPosition + 100
        var maxPosition = await _db.Lists
            .Where(l => l.BoardId == request.BoardId)
            .Select(l => (double?)l.Position)
            .MaxAsync() ?? 0.0;

        var list = new Models.List
        {
            Name = request.Name.Trim(),
            BoardId = request.BoardId,
            CreatedBy = request.CreatedBy,
            Position = maxPosition + 100.0,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Lists.Add(list);
        await _db.SaveChangesAsync();
        return list;
    }

    public async Task<Models.List?> GetListByIdAsync(int listId)
    {
        return await _db.Lists.FindAsync(listId);
    }

    public async Task<List<Models.List>> GetListsByBoardAsync(int boardId)
    {
        return await _db.Lists
            .Where(l => l.BoardId == boardId && !l.IsArchived)
            .OrderBy(l => l.Position)
            .ToListAsync();
    }

    public async Task<Models.List?> UpdateListAsync(int listId, UpdateListRequest request)
    {
        var list = await _db.Lists.FindAsync(listId);
        if (list == null)
            return null;

        list.Name = request.Name.Trim();
        list.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return list;
    }

    public async Task<Models.List?> MoveListAsync(int listId, MoveListRequest request)
    {
        var list = await _db.Lists.FindAsync(listId);
        if (list == null)
            return null;

        list.Position = request.NewPosition;
        list.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return list;
    }

    public async Task<bool> ArchiveListAsync(int listId)
    {
        var list = await _db.Lists.FindAsync(listId);
        if (list == null || list.IsArchived)
            return false;

        list.IsArchived = true;
        list.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreListAsync(int listId)
    {
        var list = await _db.Lists.FindAsync(listId);
        if (list == null || !list.IsArchived)
            return false;

        list.IsArchived = false;
        list.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}