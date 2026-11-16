using LittleTalk.Shared;
using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Auth;

namespace LittleTalk.Domain.Features.Auth.Services;

public interface IAuthService
{
    Task<Result<LoginResponseModel>> LoginAsync(
        LoginRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<UserProfileResponseModel>> GetCurrentUserProfileAsync(
        int userId, 
        CancellationToken cancellationToken = default);
}


