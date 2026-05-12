using FlowBoard.List.DTOs;
using FlowBoard.List.Models;

namespace FlowBoard.List.Services;

public interface IListService
{
    Task<Models.List> CreateListAsync(CreateListRequest request);
    Task<Models.List?> GetListByIdAsync(int listId);
    Task<List<Models.List>> GetListsByBoardAsync(int boardId);
    Task<Models.List?> UpdateListAsync(int listId, UpdateListRequest request);
    Task<Models.List?> MoveListAsync(int listId, MoveListRequest request);
    Task<bool> ArchiveListAsync(int listId);
    Task<bool> RestoreListAsync(int listId);
}