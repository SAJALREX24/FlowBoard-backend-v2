using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Workspace.DTOs;

public class AddMemberRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(20)]
    [RegularExpression("^(ADMIN|MEMBER)$", ErrorMessage = "Role must be ADMIN or MEMBER")]
    public string Role { get; set; } = "MEMBER";
}
