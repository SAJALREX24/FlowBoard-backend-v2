using System.ComponentModel.DataAnnotations;

namespace FlowBoard.Comment.DTOs;

public class UpdateCommentRequest
{
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}