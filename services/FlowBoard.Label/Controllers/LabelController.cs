using FlowBoard.Label.DTOs;
using FlowBoard.Label.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.Label.Controllers;

[ApiController]
[Route("api/labels")]
public class LabelController : ControllerBase
{
    private readonly ILabelService _labelService;

    public LabelController(ILabelService labelService)
    {
        _labelService = labelService;
    }

    // ===== Labels =====

    [HttpPost]
    public async Task<IActionResult> CreateLabel([FromBody] CreateLabelRequest request)
    {
        var label = await _labelService.CreateLabelAsync(request);
        return CreatedAtAction(nameof(GetLabelById), new { labelId = label.LabelId }, label);
    }

    [HttpGet("{labelId:int}")]
    public async Task<IActionResult> GetLabelById(int labelId)
    {
        var label = await _labelService.GetLabelByIdAsync(labelId);
        if (label == null)
            return NotFound(new { message = "Label not found." });

        return Ok(label);
    }

    [HttpGet("by-board/{boardId:int}")]
    public async Task<IActionResult> GetLabelsByBoard(int boardId)
    {
        var labels = await _labelService.GetLabelsByBoardAsync(boardId);
        return Ok(labels);
    }

    [HttpPut("{labelId:int}")]
    public async Task<IActionResult> UpdateLabel(int labelId, [FromBody] UpdateLabelRequest request)
    {
        var updated = await _labelService.UpdateLabelAsync(labelId, request);
        if (updated == null)
            return NotFound(new { message = "Label not found." });

        return Ok(updated);
    }

    [HttpDelete("{labelId:int}")]
    public async Task<IActionResult> DeleteLabel(int labelId)
    {
        var deleted = await _labelService.DeleteLabelAsync(labelId);
        if (!deleted)
            return NotFound(new { message = "Label not found." });

        return NoContent();
    }

    // ===== Checklist Items =====

    [HttpPost("checklist-items")]
    public async Task<IActionResult> CreateChecklistItem([FromBody] CreateChecklistItemRequest request)
    {
        var item = await _labelService.CreateChecklistItemAsync(request);
        return Ok(item);
    }

    [HttpGet("checklist-items/by-card/{cardId:int}")]
    public async Task<IActionResult> GetChecklistItemsByCard(int cardId)
    {
        var items = await _labelService.GetChecklistItemsByCardAsync(cardId);
        return Ok(items);
    }

    [HttpPut("checklist-items/{itemId:int}")]
    public async Task<IActionResult> UpdateChecklistItem(int itemId, [FromBody] UpdateChecklistItemRequest request)
    {
        var updated = await _labelService.UpdateChecklistItemAsync(itemId, request);
        if (updated == null)
            return NotFound(new { message = "Checklist item not found." });

        return Ok(updated);
    }

    [HttpDelete("checklist-items/{itemId:int}")]
    public async Task<IActionResult> DeleteChecklistItem(int itemId)
    {
        var deleted = await _labelService.DeleteChecklistItemAsync(itemId);
        if (!deleted)
            return NotFound(new { message = "Checklist item not found." });

        return NoContent();
    }
}