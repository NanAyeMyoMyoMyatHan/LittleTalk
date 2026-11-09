using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Auth;

namespace LittleTalk.Application.Features.Auth.Services;

public interface IAuthService
{
    Task<Result<LoginResponseModel>> LoginAsync(
        LoginRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<UserProfileResponseModel>> GetCurrentUserProfileAsync(
        int userId, 
        CancellationToken cancellationToken = default);
}
