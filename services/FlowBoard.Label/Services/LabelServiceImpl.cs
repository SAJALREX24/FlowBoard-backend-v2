using FlowBoard.Label.Data;
using FlowBoard.Label.DTOs;
using FlowBoard.Label.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Label.Services;

public class LabelServiceImpl : ILabelService
{
    private readonly LabelDbContext _db;

    public LabelServiceImpl(LabelDbContext db)
    {
        _db = db;
    }

    // ===== Labels =====

    public async Task<Models.Label> CreateLabelAsync(CreateLabelRequest request)
    {
        var label = new Models.Label
        {
            BoardId = request.BoardId,
            Name = request.Name.Trim(),
            Color = request.Color.ToUpperInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Labels.Add(label);
        await _db.SaveChangesAsync();
        return label;
    }

    public async Task<Models.Label?> GetLabelByIdAsync(int labelId)
    {
        return await _db.Labels.FindAsync(labelId);
    }

    public async Task<List<Models.Label>> GetLabelsByBoardAsync(int boardId)
    {
        return await _db.Labels
            .Where(l => l.BoardId == boardId)
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Models.Label?> UpdateLabelAsync(int labelId, UpdateLabelRequest request)
    {
        var label = await _db.Labels.FindAsync(labelId);
        if (label == null)
            return null;

        label.Name = request.Name.Trim();
        label.Color = request.Color.ToUpperInvariant();
        await _db.SaveChangesAsync();
        return label;
    }

    public async Task<bool> DeleteLabelAsync(int labelId)
    {
        var label = await _db.Labels.FindAsync(labelId);
        if (label == null)
            return false;

        _db.Labels.Remove(label);
        await _db.SaveChangesAsync();
        return true;
    }

    // ===== Checklist Items =====

    public async Task<ChecklistItem> CreateChecklistItemAsync(CreateChecklistItemRequest request)
    {
        // Auto-position at end of card's checklist
        var maxPosition = await _db.ChecklistItems
            .Where(i => i.CardId == request.CardId)
            .Select(i => (double?)i.Position)
            .MaxAsync() ?? 0.0;

        var item = new ChecklistItem
        {
            CardId = request.CardId,
            Text = request.Text.Trim(),
            IsCompleted = false,
            Position = maxPosition + 100.0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.ChecklistItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<List<ChecklistItem>> GetChecklistItemsByCardAsync(int cardId)
    {
        return await _db.ChecklistItems
            .Where(i => i.CardId == cardId)
            .OrderBy(i => i.Position)
            .ToListAsync();
    }

    public async Task<ChecklistItem?> UpdateChecklistItemAsync(int itemId, UpdateChecklistItemRequest request)
    {
        var item = await _db.ChecklistItems.FindAsync(itemId);
        if (item == null)
            return null;

        item.Text = request.Text.Trim();
        item.IsCompleted = request.IsCompleted;
        item.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteChecklistItemAsync(int itemId)
    {
        var item = await _db.ChecklistItems.FindAsync(itemId);
        if (item == null)
            return false;

        _db.ChecklistItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }
}