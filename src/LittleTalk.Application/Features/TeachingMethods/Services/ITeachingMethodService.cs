using LittleTalk.Model.Common;
using LittleTalk.Model.Features.TeachingMethods;

namespace LittleTalk.Application.Features.TeachingMethods.Services;

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
