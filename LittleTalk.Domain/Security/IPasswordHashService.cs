using LittleTalk.Shared;
using LittleTalk.Models.Common;
namespace LittleTalk.Domain.Security;

public interface IPasswordHashService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}


