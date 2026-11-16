using LittleTalk.Shared; using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Reports;

namespace LittleTalk.App.Services.Features;

public interface IReportApiClient
{
    Task<Result<StudentProgressReportResponseModel>> GetStudentProgressReportAsync(int studentId, CancellationToken cancellationToken = default);
}

