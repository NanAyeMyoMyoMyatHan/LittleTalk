using LittleTalk.Shared;
using LittleTalk.Models.Common;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using LittleTalk.Domain.Features.Auth.Services;
using LittleTalk.Domain.Features.Students;
using LittleTalk.Domain.Features.Students.Services;
using LittleTalk.Domain.Security;
using LittleTalk.Domain.Services;

namespace LittleTalk.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // 1. Validation Policies
        services.AddScoped<LittleTalk.Domain.Features.Auth.AuthValidationPolicy>();
        services.AddScoped<StudentValidationPolicy>();
        services.AddScoped<LittleTalk.Domain.Features.Courses.CourseValidationPolicy>();
        services.AddScoped<LittleTalk.Domain.Features.LessonPlans.LessonPlanValidationPolicy>();
        services.AddScoped<LittleTalk.Domain.Features.TeachingMethods.TeachingMethodValidationPolicy>();
        services.AddScoped<LittleTalk.Domain.Features.Activities.ActivityValidationPolicy>();
        services.AddScoped<LittleTalk.Domain.Features.Attendance.AttendanceValidationPolicy>();
        services.AddScoped<LittleTalk.Domain.Features.Assessments.AssessmentValidationPolicy>();

        // 2. Feature Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<LittleTalk.Domain.Features.Courses.Services.ICourseService, LittleTalk.Domain.Features.Courses.Services.CourseService>();
        services.AddScoped<LittleTalk.Domain.Features.LessonPlans.Services.ILessonPlanService, LittleTalk.Domain.Features.LessonPlans.Services.LessonPlanService>();
        services.AddScoped<LittleTalk.Domain.Features.LessonPlans.Services.IAiLessonPlanGeneratorService, LittleTalk.Domain.Features.LessonPlans.Services.AiLessonPlanGeneratorService>();
        services.AddScoped<LittleTalk.Domain.Features.TeachingMethods.Services.ITeachingMethodService, LittleTalk.Domain.Features.TeachingMethods.Services.TeachingMethodService>();
        services.AddScoped<LittleTalk.Domain.Features.Activities.Services.IActivityService, LittleTalk.Domain.Features.Activities.Services.ActivityService>();
        services.AddScoped<LittleTalk.Domain.Features.Attendance.Services.IAttendanceService, LittleTalk.Domain.Features.Attendance.Services.AttendanceService>();
        services.AddScoped<LittleTalk.Domain.Features.Assessments.Services.IAssessmentService, LittleTalk.Domain.Features.Assessments.Services.AssessmentService>();
        services.AddScoped<LittleTalk.Domain.Features.Reports.Services.IReportService, LittleTalk.Domain.Features.Reports.Services.ReportService>();
        services.AddScoped<LittleTalk.Domain.Features.Dashboard.Services.IDashboardService, LittleTalk.Domain.Features.Dashboard.Services.DashboardService>();

        // 3. Security and User Services
        services.AddHttpContextAccessor();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        => services.AddDomainServices();
}

