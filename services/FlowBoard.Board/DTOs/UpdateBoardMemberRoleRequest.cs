using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Board.DTOs;

public class UpdateBoardMemberRoleRequest
{
    [Required]
    [RegularExpression("^(OBSERVER|MEMBER|ADMIN)$", ErrorMessage = "Role must be OBSERVER, MEMBER, or ADMIN")]
    public string Role { get; set; } = string.Empty;
}
