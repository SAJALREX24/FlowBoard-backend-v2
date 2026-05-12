using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Auth.Models;

[Table("Users")]
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Role { get; set; } = "Member";

    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    [MaxLength(30)]
    public string? Provider { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}