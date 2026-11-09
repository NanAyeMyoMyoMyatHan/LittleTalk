using LittleTalk.Model.Common;
using LittleTalk.Model.Features.TeachingMethods;

namespace LittleTalk.Client.Services.Features;

public class TeachingMethodApiClient : BaseFeatureApiClient, ITeachingMethodApiClient
{
    public TeachingMethodApiClient(HttpClient http) : base(http)
    {
    }

    public async Task<Result<List<TeachingMethodResponseModel>>?> GetAllMethodsAsync(bool? isActive = null)
    {
        var query = "api/teaching-methods";
        if (isActive.HasValue)
            query += $"?isActive={isActive.Value}";

        return await SendGetAsync<List<TeachingMethodResponseModel>>(query);
    }

    public async Task<Result<TeachingMethodResponseModel>?> GetMethodByIdAsync(int id)
    {
        return await SendGetAsync<TeachingMethodResponseModel>($"api/teaching-methods/{id}");
    }

    public async Task<Result<TeachingMethodResponseModel>?> CreateMethodAsync(CreateTeachingMethodRequestModel request)
    {
        return await SendPostAsync<TeachingMethodResponseModel, CreateTeachingMethodRequestModel>("api/teaching-methods", request);
    }

    public async Task<Result<TeachingMethodResponseModel>?> UpdateMethodAsync(int id, UpdateTeachingMethodRequestModel request)
    {
        return await SendPutAsync<TeachingMethodResponseModel, UpdateTeachingMethodRequestModel>($"api/teaching-methods/{id}", request);
    }

    public async Task<Result?> DeleteMethodAsync(int id)
    {
        return await SendDeleteAsync($"api/teaching-methods/{id}");
    }
}
