using FlowBoard.Label.DTOs;
using FlowBoard.Label.Models;

namespace FlowBoard.Label.Services;

public interface ILabelService
{
    // Labels
    Task<Models.Label> CreateLabelAsync(CreateLabelRequest request);
    Task<Models.Label?> GetLabelByIdAsync(int labelId);
    Task<List<Models.Label>> GetLabelsByBoardAsync(int boardId);
    Task<Models.Label?> UpdateLabelAsync(int labelId, UpdateLabelRequest request);
    Task<bool> DeleteLabelAsync(int labelId);

    // Checklist items
    Task<ChecklistItem> CreateChecklistItemAsync(CreateChecklistItemRequest request);
    Task<List<ChecklistItem>> GetChecklistItemsByCardAsync(int cardId);
    Task<ChecklistItem?> UpdateChecklistItemAsync(int itemId, UpdateChecklistItemRequest request);
    Task<bool> DeleteChecklistItemAsync(int itemId);
}