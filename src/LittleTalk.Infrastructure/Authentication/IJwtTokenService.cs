using LittleTalk.Infrastructure.Data.Entities;

namespace LittleTalk.Infrastructure.Authentication;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
