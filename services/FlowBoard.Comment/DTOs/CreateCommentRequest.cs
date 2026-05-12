using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Comment.DTOs;

public class CreateCommentRequest
{
    [Required]
    public int CardId { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public int? ParentCommentId { get; set; }
}