namespace FlowBoard.Auth.DTOs
{
    public class UserSummaryDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}