using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Board.DTOs;

public class UpdateBoardRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(10)]
    [RegularExpression("^(PUBLIC|PRIVATE)$", ErrorMessage = "Visibility must be PUBLIC or PRIVATE")]
    public string Visibility { get; set; } = "PRIVATE";

    [MaxLength(20)]
    public string? BackgroundColor { get; set; }
}