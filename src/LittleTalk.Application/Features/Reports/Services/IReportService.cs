using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Reports;

namespace LittleTalk.Application.Features.Reports.Services;

public interface IReportService
{
    Task<Result<StudentProgressReportResponseModel>> GetStudentProgressReportAsync(int studentId, CancellationToken cancellationToken = default);
}
