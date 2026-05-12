using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Board.Models;

[Table("Boards")]
public class Board
{
    [Key]
    public int BoardId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int WorkspaceId { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    [MaxLength(10)]
    public string Visibility { get; set; } = "PRIVATE";

    [MaxLength(20)]
    public string? BackgroundColor { get; set; }

    public bool IsClosed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}