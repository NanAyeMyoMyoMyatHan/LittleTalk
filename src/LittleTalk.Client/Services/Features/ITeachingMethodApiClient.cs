using LittleTalk.Model.Common;
using LittleTalk.Model.Features.TeachingMethods;

namespace LittleTalk.Client.Services.Features;

public interface ITeachingMethodApiClient
{
    Task<Result<List<TeachingMethodResponseModel>>?> GetAllMethodsAsync(bool? isActive = null);
    Task<Result<TeachingMethodResponseModel>?> GetMethodByIdAsync(int id);
    Task<Result<TeachingMethodResponseModel>?> CreateMethodAsync(CreateTeachingMethodRequestModel request);
    Task<Result<TeachingMethodResponseModel>?> UpdateMethodAsync(int id, UpdateTeachingMethodRequestModel request);
    Task<Result?> DeleteMethodAsync(int id);
}
