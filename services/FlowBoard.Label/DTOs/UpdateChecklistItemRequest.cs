using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Label.DTOs;

public class UpdateChecklistItemRequest
{
    [Required]
    [MaxLength(200)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public bool IsCompleted { get; set; }
}