using System.Reflection;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;
using LittleTalk.Client.Components;
using LittleTalk.Client.Services;

var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? "LittleTalk.Client";
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
    Log.Information("[LittleTalk.Client] Main => Application starting up.");

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

    // 3. API Clients & HTTP Message Handler
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5131/";
    builder.Services.AddTransient<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.IAuthApiClient, LittleTalk.Client.Services.Features.AuthApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.IStudentApiClient, LittleTalk.Client.Services.Features.StudentApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.ICourseApiClient, LittleTalk.Client.Services.Features.CourseApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.ILessonPlanApiClient, LittleTalk.Client.Services.Features.LessonPlanApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.ITeachingMethodApiClient, LittleTalk.Client.Services.Features.TeachingMethodApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.IActivityApiClient, LittleTalk.Client.Services.Features.ActivityApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.IAttendanceApiClient, LittleTalk.Client.Services.Features.AttendanceApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.IAssessmentApiClient, LittleTalk.Client.Services.Features.AssessmentApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.IDashboardApiClient, LittleTalk.Client.Services.Features.DashboardApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

    builder.Services.AddHttpClient<LittleTalk.Client.Services.Features.IReportApiClient, LittleTalk.Client.Services.Features.ReportApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddHttpMessageHandler<TokenAuthenticationHandler>();

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
    Log.Fatal(ex, "[LittleTalk.Client] Main => Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
