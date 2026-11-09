using LittleTalk.Infrastructure.Authentication;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class PasswordHasherTests
{
    private readonly PasswordHashService _hasher = new();

    [Fact]
    public void HashPassword_ShouldGenerateNonEmptyHash()
    {
        // Act
        var hash = _hasher.HashPassword("SecretPassword123!");

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.True(hash.Length > 20);
    }

    [Fact]
    public void HashPassword_ShouldGenerateDifferentHashes_ForSamePasswordDueToRandomSalt()
    {
        // Arrange
        var password = "MyTestPassword123!";

        // Act
        var hash1 = _hasher.HashPassword(password);
        var hash2 = _hasher.HashPassword(password);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
    {
        // Arrange
        var password = "CorrectPassword@2026";
        var hash = _hasher.HashPassword(password);

        // Act
        var result = _hasher.VerifyPassword(hash, password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_ForIncorrectPassword()
    {
        // Arrange
        var password = "CorrectPassword@2026";
        var hash = _hasher.HashPassword(password);

        // Act
        var result = _hasher.VerifyPassword(hash, "WrongPassword@2026");

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid_hash_string")]
    public void VerifyPassword_ShouldReturnFalse_ForMalformedHash(string malformedHash)
    {
        // Act
        var result = _hasher.VerifyPassword(malformedHash, "SomePassword123");

        // Assert
        Assert.False(result);
    }
}
