using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LittleTalk.Infrastructure.Authentication;
using LittleTalk.Infrastructure.Data.Entities;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class JwtTokenServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly JwtTokenService _jwtService;

    public JwtTokenServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "JwtSettings:Secret", JwtTokenService.DefaultSecret },
            { "JwtSettings:Issuer", JwtTokenService.DefaultIssuer },
            { "JwtSettings:Audience", JwtTokenService.DefaultAudience },
            { "JwtSettings:ExpiryMinutes", "120" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _jwtService = new JwtTokenService(_configuration);
    }

    [Fact]
    public void GenerateToken_ShouldCreateValidJwtWithExpectedClaims()
    {
        // Arrange
        var testUser = new User
        {
            UserId = 42,
            Username = "teacher.sarah",
            Email = "sarah@littletalk.edu",
            FullName = "Sarah Jenkins",
            Role = new Role { RoleId = 2, RoleName = "Teacher" }
        };

        // Act
        var tokenString = _jwtService.GenerateToken(testUser);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(tokenString));

        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(tokenString));

        var jwtToken = handler.ReadJwtToken(tokenString);

        Assert.Equal(JwtTokenService.DefaultIssuer, jwtToken.Issuer);
        Assert.Contains(JwtTokenService.DefaultAudience, jwtToken.Audiences);

        // Verify claims (JwtSecurityTokenHandler maps standard types to JWT short names: nameid, unique_name, role)
        Assert.Equal("42", jwtToken.Claims.First(c => c.Type is "userId" or "nameid").Value);
        Assert.Equal("teacher.sarah", jwtToken.Claims.First(c => c.Type is "unique_name").Value);
        Assert.Equal("Teacher", jwtToken.Claims.First(c => c.Type is "role").Value);
        Assert.Equal("Sarah Jenkins", jwtToken.Claims.First(c => c.Type is "fullName").Value);
        Assert.Equal("sarah@littletalk.edu", jwtToken.Claims.First(c => c.Type is "email").Value);

        // Verify expiry is in future
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
    }
}
