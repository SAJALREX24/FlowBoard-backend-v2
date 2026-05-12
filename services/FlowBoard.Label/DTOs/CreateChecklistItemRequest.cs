using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Label.DTOs;

public class CreateChecklistItemRequest
{
    [Required]
    public int CardId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Text { get; set; } = string.Empty;
}