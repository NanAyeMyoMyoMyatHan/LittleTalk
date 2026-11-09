using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Courses;

namespace LittleTalk.Client.Services.Features;

public interface ICourseApiClient
{
    Task<Result<PagedListResponseModel<CourseSummaryResponseModel>>?> GetCoursesAsync(SearchCourseRequestModel request);
    Task<Result<CourseDetailResponseModel>?> GetCourseByIdAsync(int id);
    Task<Result<CourseDetailResponseModel>?> CreateCourseAsync(CreateCourseRequestModel request);
    Task<Result<CourseDetailResponseModel>?> UpdateCourseAsync(int id, UpdateCourseRequestModel request);
    Task<Result?> DeleteCourseAsync(int id);
    Task<Result<CourseTeacherResponseModel>?> AssignTeacherAsync(int courseId, AssignTeacherRequestModel request);
    Task<Result?> RemoveTeacherAsync(int courseId, int teacherId);
    Task<Result<List<CourseEnrollmentResponseModel>>?> GetCourseEnrollmentsAsync(int courseId);
    Task<Result<CourseEnrollmentResponseModel>?> EnrollStudentAsync(int courseId, EnrollStudentRequestModel request);
    Task<Result<CourseEnrollmentResponseModel>?> UpdateEnrollmentStatusAsync(int courseId, int studentId, UpdateEnrollmentStatusRequestModel request);
}
