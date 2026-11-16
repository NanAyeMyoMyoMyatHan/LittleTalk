using LittleTalk.Shared; using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Reports;

namespace LittleTalk.App.Services.Features;

public class ReportApiClient : BaseFeatureApiClient, IReportApiClient
{
    public ReportApiClient(HttpClient http, CustomAuthenticationStateProvider authStateProvider) 
        : base(http, authStateProvider)
    {
    }

    public async Task<Result<StudentProgressReportResponseModel>> GetStudentProgressReportAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<StudentProgressReportResponseModel>($"api/reports/student/{studentId}"))
               ?? Result<StudentProgressReportResponseModel>.SystemError("No response received from server.");
    }
}

