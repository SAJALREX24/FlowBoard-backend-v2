using FlowBoard.Card.DTOs;
using FlowBoard.Card.Models;

namespace FlowBoard.Card.Services;

public interface ICardService
{
    Task<Models.Card> CreateCardAsync(CreateCardRequest request);
    Task<Models.Card?> GetCardByIdAsync(int cardId);
    Task<List<Models.Card>> GetCardsByListAsync(int listId);
    Task<List<Models.Card>> GetCardsByBoardAsync(int boardId);
    Task<List<Models.Card>> GetCardsByAssigneeAsync(int userId);
    Task<List<Models.Card>> GetOverdueCardsAsync();
    Task<Models.Card?> UpdateCardAsync(int cardId, UpdateCardRequest request);
    Task<Models.Card?> MoveCardAsync(int cardId, MoveCardRequest request);
    Task<Models.Card?> AssignCardAsync(int cardId, AssignCardRequest request);
    Task<bool> ArchiveCardAsync(int cardId);
    Task<bool> RestoreCardAsync(int cardId);
}