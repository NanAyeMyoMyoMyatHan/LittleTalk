using LittleTalk.Shared;
using LittleTalk.Models.Common;
using LittleTalk.Database.Entities;

namespace LittleTalk.Domain.Security;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}


