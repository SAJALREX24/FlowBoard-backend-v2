using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Card.DTOs;

public class MoveCardRequest
{
    [Required]
    public int NewListId { get; set; }

    [Required]
    public double NewPosition { get; set; }
}