using FlowBoard.Auth.DTOs;
using FlowBoard.Auth.Models;

namespace FlowBoard.Auth.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(int userId);
}