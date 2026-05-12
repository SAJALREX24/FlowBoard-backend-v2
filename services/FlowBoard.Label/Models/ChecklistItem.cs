using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Label.Models;

[Table("ChecklistItems")]
public class ChecklistItem
{
    [Key]
    public int ItemId { get; set; }

    [Required]
    public int CardId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Text { get; set; } = string.Empty;

    public bool IsCompleted { get; set; } = false;

    [Required]
    public double Position { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}