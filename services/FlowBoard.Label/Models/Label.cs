using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Label.Models;

[Table("Labels")]
public class Label
{
    [Key]
    public int LabelId { get; set; }

    [Required]
    public int BoardId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Color { get; set; } = "GRAY";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}