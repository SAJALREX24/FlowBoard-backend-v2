using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.List.Models;

[Table("Lists")]
public class List
{
    [Key]
    public int ListId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int BoardId { get; set; }

    [Required]
    public double Position { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    public bool IsArchived { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}