using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Board.Models;

[Table("BoardMembers")]
public class BoardMember
{
    [Key]
    public int MemberId { get; set; }

    [Required]
    public int BoardId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "MEMBER";

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}