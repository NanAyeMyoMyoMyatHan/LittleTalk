using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Courses;

namespace LittleTalk.Application.Features.Courses.Services;

public interface ICourseService
{
    Task<Result<PagedListResponseModel<CourseSummaryResponseModel>>> GetCoursesAsync(
        SearchCourseRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<CourseDetailResponseModel>> GetCourseByIdAsync(
        int courseId, 
        CancellationToken cancellationToken = default);

    Task<Result<CourseDetailResponseModel>> CreateCourseAsync(
        CreateCourseRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<CourseDetailResponseModel>> UpdateCourseAsync(
        int courseId, 
        UpdateCourseRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteCourseAsync(
        int courseId, 
        CancellationToken cancellationToken = default);

    Task<Result<CourseTeacherResponseModel>> AssignTeacherAsync(
        int courseId, 
        AssignTeacherRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> RemoveTeacherAsync(
        int courseId, 
        int teacherId, 
        CancellationToken cancellationToken = default);

    Task<Result<List<CourseEnrollmentResponseModel>>> GetCourseEnrollmentsAsync(
        int courseId, 
        CancellationToken cancellationToken = default);

    Task<Result<CourseEnrollmentResponseModel>> EnrollStudentAsync(
        int courseId, 
        EnrollStudentRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<CourseEnrollmentResponseModel>> UpdateEnrollmentStatusAsync(
        int courseId, 
        int studentId, 
        UpdateEnrollmentStatusRequestModel request, 
        CancellationToken cancellationToken = default);
}
