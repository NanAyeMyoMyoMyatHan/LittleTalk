using LittleTalk.Shared;
using LittleTalk.Models.Common;
namespace LittleTalk.Domain.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    string? Role { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}


