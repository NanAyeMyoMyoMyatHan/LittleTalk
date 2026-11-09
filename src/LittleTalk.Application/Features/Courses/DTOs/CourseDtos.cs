namespace LittleTalk.Application.Features.Courses.DTOs;

public class CourseSummaryDto
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

public class CourseDetailDto : CourseSummaryDto
{
    public List<CourseTeacherDto> Teachers { get; set; } = new();
    public List<CourseEnrollmentSummaryDto> EnrolledStudents { get; set; } = new();
    public DateTime? UpdatedDate { get; set; }
}

public class CourseTeacherDto
{
    public int CourseTeacherId { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string TeacherEmail { get; set; } = string.Empty;
    public string RoleInCourse { get; set; } = string.Empty; // Primary, CoTeacher, Assistant, Substitute
    public bool IsActive { get; set; }
    public DateTime AssignedDate { get; set; }
}

public class CourseEnrollmentSummaryDto
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

public class CreateCourseDto
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int LearningLevelId { get; set; }
    public int? DurationWeeks { get; set; }
    public int MaxStudents { get; set; } = 15;
    public string Status { get; set; } = "Active";
    public int? PrimaryTeacherId { get; set; } // Optional initial lead teacher
}

public class UpdateCourseDto
{
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int LearningLevelId { get; set; }
    public int? DurationWeeks { get; set; }
    public int MaxStudents { get; set; } = 15;
    public string Status { get; set; } = "Active";
}

public class AssignTeacherToCourseDto
{
    public int TeacherId { get; set; }
    public string RoleInCourse { get; set; } = "CoTeacher"; // Primary, CoTeacher, Assistant, Substitute
}

public class EnrollStudentDto
{
    public int StudentId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateEnrollmentStatusDto
{
    public string Status { get; set; } = "Enrolled"; // Enrolled, Completed, Dropped, Transferred
    public string? Notes { get; set; }
}
