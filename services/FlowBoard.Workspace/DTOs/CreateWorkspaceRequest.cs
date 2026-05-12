using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Workspace.DTOs;

public class CreateWorkspaceRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required]
    [MaxLength(10)]
    [RegularExpression("^(PUBLIC|PRIVATE)$", ErrorMessage = "Visibility must be PUBLIC or PRIVATE")]
    public string Visibility { get; set; } = "PRIVATE";

    [MaxLength(500)]
    public string? LogoUrl { get; set; }
}