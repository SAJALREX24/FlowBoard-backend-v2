using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Board.DTOs;

public class AddBoardMemberRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(20)]
    [RegularExpression("^(OBSERVER|MEMBER|ADMIN)$", ErrorMessage = "Role must be OBSERVER, MEMBER, or ADMIN")]
    public string Role { get; set; } = "MEMBER";
}
