using LittleTalk.Shared; using LittleTalk.Models.Common;
using LittleTalk.Models.Features.TeachingMethods;

namespace LittleTalk.App.Services.Features;

public interface ITeachingMethodApiClient
{
    Task<Result<List<TeachingMethodResponseModel>>?> GetAllMethodsAsync(bool? isActive = null);
    Task<Result<TeachingMethodResponseModel>?> GetMethodByIdAsync(int id);
    Task<Result<TeachingMethodResponseModel>?> CreateMethodAsync(CreateTeachingMethodRequestModel request);
    Task<Result<TeachingMethodResponseModel>?> UpdateMethodAsync(int id, UpdateTeachingMethodRequestModel request);
    Task<Result?> DeleteMethodAsync(int id);
}

