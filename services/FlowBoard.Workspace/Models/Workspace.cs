using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Workspace.Models;

[Table("Workspaces")]
public class Workspace
{
    [Key]
    public int WorkspaceId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required]
    [MaxLength(10)]
    public string Visibility { get; set; } = "PRIVATE";

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}