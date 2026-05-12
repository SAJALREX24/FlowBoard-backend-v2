using System.ComponentModel.DataAnnotations;

namespace FlowBoard.List.DTOs;

public class UpdateListRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}