using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Workspace.Models;

[Table("WorkspaceMembers")]
public class WorkspaceMember
{
    [Key]
    public int MemberId { get; set; }

    [Required]
    public int WorkspaceId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "MEMBER";

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}