using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Card.Models;

[Table("Cards")]
public class Card
{
    [Key]
    public int CardId { get; set; }

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
    public double Position { get; set; }

    public int? AssigneeId { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    public DateTime? DueDate { get; set; }

    [MaxLength(20)]
    public string? Priority { get; set; }

    [MaxLength(20)]
    public string? CoverColor { get; set; }

    public bool IsArchived { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}