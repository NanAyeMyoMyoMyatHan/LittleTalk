using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Courses;

namespace LittleTalk.Client.Services.Features;

public class CourseApiClient : BaseFeatureApiClient, ICourseApiClient
{
    public CourseApiClient(HttpClient http) : base(http)
    {
    }

    public async Task<Result<PagedListResponseModel<CourseSummaryResponseModel>>?> GetCoursesAsync(SearchCourseRequestModel request)
    {
        var query = $"api/courses?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query += $"&searchTerm={Uri.EscapeDataString(request.SearchTerm)}";
        var levelId = request.LearningLevelId ?? request.LevelId;
        if (levelId.HasValue)
            query += $"&levelId={levelId.Value}";
        if (!string.IsNullOrWhiteSpace(request.Status))
            query += $"&status={Uri.EscapeDataString(request.Status)}";

        return await SendGetAsync<PagedListResponseModel<CourseSummaryResponseModel>>(query);
    }

    public async Task<Result<CourseDetailResponseModel>?> GetCourseByIdAsync(int id)
    {
        return await SendGetAsync<CourseDetailResponseModel>($"api/courses/{id}");
    }

    public async Task<Result<CourseDetailResponseModel>?> CreateCourseAsync(CreateCourseRequestModel request)
    {
        return await SendPostAsync<CourseDetailResponseModel, CreateCourseRequestModel>("api/courses", request);
    }

    public async Task<Result<CourseDetailResponseModel>?> UpdateCourseAsync(int id, UpdateCourseRequestModel request)
    {
        return await SendPutAsync<CourseDetailResponseModel, UpdateCourseRequestModel>($"api/courses/{id}", request);
    }

    public async Task<Result?> DeleteCourseAsync(int id)
    {
        return await SendDeleteAsync($"api/courses/{id}");
    }

    public async Task<Result<CourseTeacherResponseModel>?> AssignTeacherAsync(int courseId, AssignTeacherRequestModel request)
    {
        return await SendPostAsync<CourseTeacherResponseModel, AssignTeacherRequestModel>($"api/courses/{courseId}/teachers", request);
    }

    public async Task<Result?> RemoveTeacherAsync(int courseId, int teacherId)
    {
        return await SendDeleteAsync($"api/courses/{courseId}/teachers/{teacherId}");
    }

    public async Task<Result<List<CourseEnrollmentResponseModel>>?> GetCourseEnrollmentsAsync(int courseId)
    {
        return await SendGetAsync<List<CourseEnrollmentResponseModel>>($"api/courses/{courseId}/enrollments");
    }

    public async Task<Result<CourseEnrollmentResponseModel>?> EnrollStudentAsync(int courseId, EnrollStudentRequestModel request)
    {
        return await SendPostAsync<CourseEnrollmentResponseModel, EnrollStudentRequestModel>($"api/courses/{courseId}/enroll", request);
    }

    public async Task<Result<CourseEnrollmentResponseModel>?> UpdateEnrollmentStatusAsync(int courseId, int studentId, UpdateEnrollmentStatusRequestModel request)
    {
        return await SendPutAsync<CourseEnrollmentResponseModel, UpdateEnrollmentStatusRequestModel>($"api/courses/{courseId}/enrollments/{studentId}/status", request);
    }
}
