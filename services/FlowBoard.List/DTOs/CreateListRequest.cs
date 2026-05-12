using System.ComponentModel.DataAnnotations;

namespace FlowBoard.List.DTOs;

public class CreateListRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int BoardId { get; set; }

    [Required]
    public int CreatedBy { get; set; }
}