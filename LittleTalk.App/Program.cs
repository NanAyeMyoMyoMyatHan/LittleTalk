using System.Reflection;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;
using LittleTalk.App.Components;
using LittleTalk.App.Services;

var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? "LittleTalk.App";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        $"logs/{assemblyName}.log",
        rollingInterval: RollingInterval.Hour)
    .CreateLogger();

try
{
    Log.Information("[LittleTalk.App] Main => Application starting up.");

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();

    // 1. MudBlazor UI Services
    builder.Services.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.NewestOnTop = true;
        config.SnackbarConfiguration.ShowCloseIcon = true;
        config.SnackbarConfiguration.VisibleStateDuration = 3500;
        config.SnackbarConfiguration.HideTransitionDuration = 300;
        config.SnackbarConfiguration.ShowTransitionDuration = 300;
        config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    });

    // 2. Client Authentication & Storage
    builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
    builder.Services.AddScoped<CustomAuthenticationStateProvider>();
    builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
    builder.Services.AddAuthorizationCore();

    // 3. API Clients
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5131/";

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.IAuthApiClient, LittleTalk.App.Services.Features.AuthApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.IStudentApiClient, LittleTalk.App.Services.Features.StudentApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.ICourseApiClient, LittleTalk.App.Services.Features.CourseApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.ILessonPlanApiClient, LittleTalk.App.Services.Features.LessonPlanApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.ITeachingMethodApiClient, LittleTalk.App.Services.Features.TeachingMethodApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.IActivityApiClient, LittleTalk.App.Services.Features.ActivityApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.IAttendanceApiClient, LittleTalk.App.Services.Features.AttendanceApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.IAssessmentApiClient, LittleTalk.App.Services.Features.AssessmentApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.IDashboardApiClient, LittleTalk.App.Services.Features.DashboardApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddHttpClient<LittleTalk.App.Services.Features.IReportApiClient, LittleTalk.App.Services.Features.ReportApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    });

    // 4. Health Checks
    builder.Services.AddHealthChecks();

    // 5. Blazor Interactive Server
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapHealthChecks("/health");

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "[LittleTalk.App] Main => Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

