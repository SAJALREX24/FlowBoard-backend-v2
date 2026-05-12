using FlowBoard.Card.Data;
using FlowBoard.Card.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Card.Services;

public class CardServiceImpl : ICardService
{
    private readonly CardDbContext _db;

    public CardServiceImpl(CardDbContext db)
    {
        _db = db;
    }

    public async Task<Models.Card> CreateCardAsync(CreateCardRequest request)
    {
        // Auto-position at end of list: maxPosition + 100
        var maxPosition = await _db.Cards
            .Where(c => c.ListId == request.ListId)
            .Select(c => (double?)c.Position)
            .MaxAsync() ?? 0.0;

        var card = new Models.Card
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            ListId = request.ListId,
            BoardId = request.BoardId,
            CreatedBy = request.CreatedBy,
            Position = maxPosition + 100.0,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Cards.Add(card);
        await _db.SaveChangesAsync();
        return card;
    }

    public async Task<Models.Card?> GetCardByIdAsync(int cardId)
    {
        return await _db.Cards.FindAsync(cardId);
    }

    public async Task<List<Models.Card>> GetCardsByListAsync(int listId)
    {
        return await _db.Cards
            .Where(c => c.ListId == listId && !c.IsArchived)
            .OrderBy(c => c.Position)
            .ToListAsync();
    }

    public async Task<List<Models.Card>> GetCardsByBoardAsync(int boardId)
    {
        return await _db.Cards
            .Where(c => c.BoardId == boardId && !c.IsArchived)
            .OrderBy(c => c.ListId)
            .ThenBy(c => c.Position)
            .ToListAsync();
    }

    public async Task<List<Models.Card>> GetCardsByAssigneeAsync(int userId)
    {
        return await _db.Cards
            .Where(c => c.AssigneeId == userId && !c.IsArchived)
            .OrderBy(c => c.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    public async Task<List<Models.Card>> GetOverdueCardsAsync()
    {
        var now = DateTime.UtcNow;
        return await _db.Cards
            .Where(c => c.DueDate != null && c.DueDate < now && !c.IsArchived)
            .OrderBy(c => c.DueDate)
            .ToListAsync();
    }

    public async Task<Models.Card?> UpdateCardAsync(int cardId, UpdateCardRequest request)
    {
        var card = await _db.Cards.FindAsync(cardId);
        if (card == null)
            return null;

        card.Title = request.Title.Trim();
        card.Description = request.Description?.Trim();
        card.AssigneeId = request.AssigneeId;
        card.DueDate = request.DueDate;
        card.Priority = request.Priority?.ToUpperInvariant();
        card.CoverColor = request.CoverColor?.Trim();
        card.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return card;
    }

    public async Task<Models.Card?> MoveCardAsync(int cardId, MoveCardRequest request)
    {
        var card = await _db.Cards.FindAsync(cardId);
        if (card == null)
            return null;

        card.ListId = request.NewListId;
        card.Position = request.NewPosition;
        card.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return card;
    }

    public async Task<Models.Card?> AssignCardAsync(int cardId, AssignCardRequest request)
    {
        var card = await _db.Cards.FindAsync(cardId);
        if (card == null)
            return null;

        card.AssigneeId = request.AssigneeId;
        card.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return card;
    }

    public async Task<bool> ArchiveCardAsync(int cardId)
    {
        var card = await _db.Cards.FindAsync(cardId);
        if (card == null || card.IsArchived)
            return false;

        card.IsArchived = true;
        card.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreCardAsync(int cardId)
    {
        var card = await _db.Cards.FindAsync(cardId);
        if (card == null || !card.IsArchived)
            return false;

        card.IsArchived = false;
        card.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}