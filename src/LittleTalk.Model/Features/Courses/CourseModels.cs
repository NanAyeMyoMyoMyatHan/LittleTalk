using LittleTalk.Model.Common;

namespace LittleTalk.Model.Features.Courses;

public class SearchCourseRequestModel : PaginationRequestModel
{
    public int? LearningLevelId { get; set; }
    public int? LevelId { get => LearningLevelId; set => LearningLevelId = value; }
    public string? Status { get; set; }
}

public class CreateCourseRequestModel
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int LearningLevelId { get; set; }
    public int? DurationWeeks { get; set; }
    public int MaxStudents { get; set; } = 15;
    public string Status { get; set; } = "Active";
    public int? PrimaryTeacherId { get; set; }
}

public class UpdateCourseRequestModel
{
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int LearningLevelId { get; set; }
    public int? DurationWeeks { get; set; }
    public int MaxStudents { get; set; } = 15;
    public string Status { get; set; } = "Active";
}

public class AssignTeacherRequestModel
{
    public int TeacherId { get; set; }
    public string RoleInCourse { get; set; } = "CoTeacher";
}

public class EnrollStudentRequestModel
{
    public int StudentId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateEnrollmentStatusRequestModel
{
    public string Status { get; set; } = "Enrolled";
    public string? Notes { get; set; }
}

public class CourseSummaryResponseModel
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int LearningLevelId { get; set; }
    public string LearningLevelName { get; set; } = string.Empty;
    public int? DurationWeeks { get; set; }
    public int MaxStudents { get; set; }
    public string Status { get; set; } = string.Empty;
    public int EnrolledStudentsCount { get; set; }
    public string? PrimaryTeacherName { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CourseDetailResponseModel : CourseSummaryResponseModel
{
    public List<CourseTeacherResponseModel> Teachers { get; set; } = new();
    public List<CourseEnrollmentResponseModel> EnrolledStudents { get; set; } = new();
    public DateTime? UpdatedDate { get; set; }
}

public class CourseTeacherResponseModel
{
    public int CourseTeacherId { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string TeacherEmail { get; set; } = string.Empty;
    public string RoleInCourse { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime AssignedDate { get; set; }
}

public class CourseEnrollmentResponseModel
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentFullName { get; set; } = string.Empty;
    public string? StudentNickName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
}
