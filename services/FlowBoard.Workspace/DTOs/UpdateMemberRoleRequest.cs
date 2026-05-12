using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Workspace.DTOs;

public class UpdateMemberRoleRequest
{
    [Required]
    [RegularExpression("^(ADMIN|MEMBER)$", ErrorMessage = "Role must be ADMIN or MEMBER")]
    public string Role { get; set; } = string.Empty;
}
