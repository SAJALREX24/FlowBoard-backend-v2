using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Card.DTOs;

public class CreateCardRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public int ListId { get; set; }

    [Required]
    public int BoardId { get; set; }

    [Required]
    public int CreatedBy { get; set; }
}