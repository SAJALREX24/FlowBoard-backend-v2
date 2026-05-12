using FlowBoard.List.DTOs;
using FlowBoard.List.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.List.Controllers;

[ApiController]
[Route("api/lists")]
public class ListController : ControllerBase
{
    private readonly IListService _listService;

    public ListController(IListService listService)
    {
        _listService = listService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListRequest request)
    {
        var list = await _listService.CreateListAsync(request);
        return CreatedAtAction(nameof(GetById), new { listId = list.ListId }, list);
    }

    [HttpGet("{listId:int}")]
    public async Task<IActionResult> GetById(int listId)
    {
        var list = await _listService.GetListByIdAsync(listId);
        if (list == null)
            return NotFound(new { message = "List not found." });

        return Ok(list);
    }

    [HttpGet("by-board/{boardId:int}")]
    public async Task<IActionResult> GetByBoard(int boardId)
    {
        var lists = await _listService.GetListsByBoardAsync(boardId);
        return Ok(lists);
    }

    [HttpPut("{listId:int}")]
    public async Task<IActionResult> Update(int listId, [FromBody] UpdateListRequest request)
    {
        var updated = await _listService.UpdateListAsync(listId, request);
        if (updated == null)
            return NotFound(new { message = "List not found." });

        return Ok(updated);
    }

    [HttpPost("{listId:int}/move")]
    public async Task<IActionResult> Move(int listId, [FromBody] MoveListRequest request)
    {
        var moved = await _listService.MoveListAsync(listId, request);
        if (moved == null)
            return NotFound(new { message = "List not found." });

        return Ok(moved);
    }

    [HttpPost("{listId:int}/archive")]
    public async Task<IActionResult> Archive(int listId)
    {
        var archived = await _listService.ArchiveListAsync(listId);
        if (!archived)
            return NotFound(new { message = "List not found or already archived." });

        return NoContent();
    }

    [HttpPost("{listId:int}/restore")]
    public async Task<IActionResult> Restore(int listId)
    {
        var restored = await _listService.RestoreListAsync(listId);
        if (!restored)
            return NotFound(new { message = "List not found or not archived." });

        return NoContent();
    }
}