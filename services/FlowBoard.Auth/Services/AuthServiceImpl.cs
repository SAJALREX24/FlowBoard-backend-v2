using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlowBoard.Auth.Data;
using FlowBoard.Auth.DTOs;
using FlowBoard.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FlowBoard.Auth.Services;

public class AuthServiceImpl : IAuthService
{
    private readonly AuthDbContext _db;
    private readonly IConfiguration _config;

    public AuthServiceImpl(AuthDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.Username.Trim();

        if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
            throw new InvalidOperationException("An account with this email already exists.");

        if (await _db.Users.AnyAsync(u => u.Username == normalizedUsername))
            throw new InvalidOperationException("This username is already taken.");

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            Username = normalizedUsername,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            Role = "Member",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return BuildAuthResponse(user);
    }




    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return BuildAuthResponse(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == normalized);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _db.Users.FindAsync(userId);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key missing");
        var jwtIssuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer missing");
        var jwtAudience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience missing");
        var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "1440");

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse
        {
            Token = tokenString,
            UserId = user.UserId,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }
}