using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Board.DTOs;

public class CreateBoardRequest
{
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
    [RegularExpression("^(PUBLIC|PRIVATE)$", ErrorMessage = "Visibility must be PUBLIC or PRIVATE")]
    public string Visibility { get; set; } = "PRIVATE";

    [MaxLength(20)]
    public string? BackgroundColor { get; set; }
}
