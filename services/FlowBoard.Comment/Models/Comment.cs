using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Comment.Models;

[Table("Comments")]
public class Comment
{
    [Key]
    public int CommentId { get; set; }

    [Required]
    public int CardId { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public int? ParentCommentId { get; set; }

    public bool IsEdited { get; set; } = false;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}