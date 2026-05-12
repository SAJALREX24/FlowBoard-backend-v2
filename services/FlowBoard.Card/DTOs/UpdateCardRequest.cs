using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Card.DTOs;

public class UpdateCardRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? AssigneeId { get; set; }

    public DateTime? DueDate { get; set; }

    [MaxLength(20)]
    [RegularExpression("^(LOW|MEDIUM|HIGH|URGENT)$", ErrorMessage = "Priority must be LOW, MEDIUM, HIGH, or URGENT")]
    public string? Priority { get; set; }

    [MaxLength(20)]
    public string? CoverColor { get; set; }
}