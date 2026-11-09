using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Dashboard;

namespace LittleTalk.Client.Services.Features;

public class DashboardApiClient : BaseFeatureApiClient, IDashboardApiClient
{
    public DashboardApiClient(HttpClient http, CustomAuthenticationStateProvider authStateProvider) 
        : base(http, authStateProvider)
    {
    }

    public async Task<Result<TeacherDashboardResponseModel>> GetTeacherDashboardAsync(CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<TeacherDashboardResponseModel>("api/dashboard/teacher"))
               ?? Result<TeacherDashboardResponseModel>.SystemError("No response received from server.");
    }

    public async Task<Result<AdminDashboardResponseModel>> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<AdminDashboardResponseModel>("api/dashboard/admin"))
               ?? Result<AdminDashboardResponseModel>.SystemError("No response received from server.");
    }
}
