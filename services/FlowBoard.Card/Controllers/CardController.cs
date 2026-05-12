using FlowBoard.Card.DTOs;
using FlowBoard.Card.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.Card.Controllers;

[ApiController]
[Route("api/cards")]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCardRequest request)
    {
        var card = await _cardService.CreateCardAsync(request);
        return CreatedAtAction(nameof(GetById), new { cardId = card.CardId }, card);
    }

    [HttpGet("{cardId:int}")]
    public async Task<IActionResult> GetById(int cardId)
    {
        var card = await _cardService.GetCardByIdAsync(cardId);
        if (card == null)
            return NotFound(new { message = "Card not found." });

        return Ok(card);
    }

    [HttpGet("by-list/{listId:int}")]
    public async Task<IActionResult> GetByList(int listId)
    {
        var cards = await _cardService.GetCardsByListAsync(listId);
        return Ok(cards);
    }

    [HttpGet("by-board/{boardId:int}")]
    public async Task<IActionResult> GetByBoard(int boardId)
    {
        var cards = await _cardService.GetCardsByBoardAsync(boardId);
        return Ok(cards);
    }

    [HttpGet("by-assignee/{userId:int}")]
    public async Task<IActionResult> GetByAssignee(int userId)
    {
        var cards = await _cardService.GetCardsByAssigneeAsync(userId);
        return Ok(cards);
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        var cards = await _cardService.GetOverdueCardsAsync();
        return Ok(cards);
    }

    [HttpPut("{cardId:int}")]
    public async Task<IActionResult> Update(int cardId, [FromBody] UpdateCardRequest request)
    {
        var updated = await _cardService.UpdateCardAsync(cardId, request);
        if (updated == null)
            return NotFound(new { message = "Card not found." });

        return Ok(updated);
    }

    [HttpPost("{cardId:int}/move")]
    public async Task<IActionResult> Move(int cardId, [FromBody] MoveCardRequest request)
    {
        var moved = await _cardService.MoveCardAsync(cardId, request);
        if (moved == null)
            return NotFound(new { message = "Card not found." });

        return Ok(moved);
    }

    [HttpPost("{cardId:int}/assign")]
    public async Task<IActionResult> Assign(int cardId, [FromBody] AssignCardRequest request)
    {
        var assigned = await _cardService.AssignCardAsync(cardId, request);
        if (assigned == null)
            return NotFound(new { message = "Card not found." });

        return Ok(assigned);
    }

    [HttpPost("{cardId:int}/archive")]
    public async Task<IActionResult> Archive(int cardId)
    {
        var archived = await _cardService.ArchiveCardAsync(cardId);
        if (!archived)
            return NotFound(new { message = "Card not found or already archived." });

        return NoContent();
    }

    [HttpPost("{cardId:int}/restore")]
    public async Task<IActionResult> Restore(int cardId)
    {
        var restored = await _cardService.RestoreCardAsync(cardId);
        if (!restored)
            return NotFound(new { message = "Card not found or not archived." });

        return NoContent();
    }
}