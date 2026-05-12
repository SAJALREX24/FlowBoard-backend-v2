using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Label.DTOs;

public class CreateLabelRequest
{
    [Required]
    public int BoardId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [RegularExpression("^(RED|ORANGE|YELLOW|GREEN|BLUE|PURPLE|PINK|GRAY)$", ErrorMessage = "Color must be one of: RED, ORANGE, YELLOW, GREEN, BLUE, PURPLE, PINK, GRAY")]
    public string Color { get; set; } = "GRAY";
}