using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LittleTalk.Infrastructure.Data.Entities;

namespace LittleTalk.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public const string DefaultSecret = "LittleTalk_SuperSecure_SecretKey_2026_KidsEnglishTeaching_ManagementSystem!";
    public const string DefaultIssuer = "LittleTalkApi";
    public const string DefaultAudience = "LittleTalkClient";

    public string GenerateToken(User user)
    {
        var secret = _configuration["JwtSettings:Secret"] ?? DefaultSecret;
        var issuer = _configuration["JwtSettings:Issuer"] ?? DefaultIssuer;
        var audience = _configuration["JwtSettings:Audience"] ?? DefaultAudience;
        var expiryMinutes = int.TryParse(_configuration["JwtSettings:ExpiryMinutes"], out var minutes) ? minutes : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.RoleName),
            new(ClaimTypes.GivenName, user.FullName),
            new("userId", user.UserId.ToString()),
            new("role", user.Role.RoleName),
            new("email", user.Email),
            new("fullName", user.FullName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
