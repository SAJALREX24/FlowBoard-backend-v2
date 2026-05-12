using System.ComponentModel.DataAnnotations;

namespace FlowBoard.List.DTOs;

public class MoveListRequest
{
    [Required]
    public double NewPosition { get; set; }
}