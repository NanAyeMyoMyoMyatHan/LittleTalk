using LittleTalk.Shared;
using LittleTalk.Models.Common;
using LittleTalk.Models.Features.TeachingMethods;

namespace LittleTalk.Domain.Features.TeachingMethods.Services;

public interface ITeachingMethodService
{
    Task<Result<List<TeachingMethodResponseModel>>> GetAllMethodsAsync(
        bool? isActive = null, 
        CancellationToken cancellationToken = default);

    Task<Result<TeachingMethodResponseModel>> GetMethodByIdAsync(
        int id, 
        CancellationToken cancellationToken = default);

    Task<Result<TeachingMethodResponseModel>> CreateMethodAsync(
        CreateTeachingMethodRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<TeachingMethodResponseModel>> UpdateMethodAsync(
        int id, 
        UpdateTeachingMethodRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteMethodAsync(
        int id, 
        CancellationToken cancellationToken = default);
}


