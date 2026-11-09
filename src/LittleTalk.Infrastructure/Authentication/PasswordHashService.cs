using Microsoft.AspNetCore.Identity;
using LittleTalk.Infrastructure.Data.Entities;

namespace LittleTalk.Infrastructure.Authentication;

public class PasswordHashService : IPasswordHashService
{
    private readonly PasswordHasher<User> _passwordHasher;
    private static readonly User DummyUser = new();

    public PasswordHashService()
    {
        _passwordHasher = new PasswordHasher<User>();
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(DummyUser, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(providedPassword))
            return false;

        try
        {
            var result = _passwordHasher.VerifyHashedPassword(DummyUser, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
