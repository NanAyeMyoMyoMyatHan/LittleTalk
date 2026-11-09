using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using LittleTalk.Application.Features.Auth.Services;
using LittleTalk.Application.Features.Students;
using LittleTalk.Application.Features.Students.Services;

namespace LittleTalk.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // 1. Validation Policies
        services.AddScoped<LittleTalk.Application.Features.Auth.AuthValidationPolicy>();
        services.AddScoped<StudentValidationPolicy>();
        services.AddScoped<LittleTalk.Application.Features.Courses.CourseValidationPolicy>();
        services.AddScoped<LittleTalk.Application.Features.LessonPlans.LessonPlanValidationPolicy>();
        services.AddScoped<LittleTalk.Application.Features.TeachingMethods.TeachingMethodValidationPolicy>();
        services.AddScoped<LittleTalk.Application.Features.Activities.ActivityValidationPolicy>();
        services.AddScoped<LittleTalk.Application.Features.Attendance.AttendanceValidationPolicy>();
        services.AddScoped<LittleTalk.Application.Features.Assessments.AssessmentValidationPolicy>();

        // 2. Feature Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<LittleTalk.Application.Features.Courses.Services.ICourseService, LittleTalk.Application.Features.Courses.Services.CourseService>();
        services.AddScoped<LittleTalk.Application.Features.LessonPlans.Services.ILessonPlanService, LittleTalk.Application.Features.LessonPlans.Services.LessonPlanService>();
        services.AddScoped<LittleTalk.Application.Features.TeachingMethods.Services.ITeachingMethodService, LittleTalk.Application.Features.TeachingMethods.Services.TeachingMethodService>();
        services.AddScoped<LittleTalk.Application.Features.Activities.Services.IActivityService, LittleTalk.Application.Features.Activities.Services.ActivityService>();
        services.AddScoped<LittleTalk.Application.Features.Attendance.Services.IAttendanceService, LittleTalk.Application.Features.Attendance.Services.AttendanceService>();
        services.AddScoped<LittleTalk.Application.Features.Assessments.Services.IAssessmentService, LittleTalk.Application.Features.Assessments.Services.AssessmentService>();
        services.AddScoped<LittleTalk.Application.Features.Reports.Services.IReportService, LittleTalk.Application.Features.Reports.Services.ReportService>();
        services.AddScoped<LittleTalk.Application.Features.Dashboard.Services.IDashboardService, LittleTalk.Application.Features.Dashboard.Services.DashboardService>();

        return services;
    }
}
