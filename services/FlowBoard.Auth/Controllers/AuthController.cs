using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using FlowBoard.Auth.DTOs;
using FlowBoard.Auth.Services;

namespace FlowBoard.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        return Ok(new
        {
            user.UserId,
            user.Email,
            user.Username,
            user.FullName,
            user.Role,
            user.AvatarUrl,
            user.IsActive,
            user.CreatedAt
        });
    }

    [HttpGet("users/by-email/{emailAddr}")]
    public async Task<IActionResult> GetUserByEmail(string emailAddr)
    {
        var user = await _authService.GetUserByEmailAsync(emailAddr);
        if (user == null)
            return NotFound(new { message = "User not found." });

        return Ok(new
        {
            user.UserId,
            user.Email,
            user.Username,
            user.FullName,
            user.Role,
            user.AvatarUrl,
            user.IsActive,
            user.CreatedAt
        });
    }
}