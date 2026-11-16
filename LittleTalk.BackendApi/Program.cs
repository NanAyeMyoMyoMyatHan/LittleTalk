using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using LittleTalk.BackendApi.Extensions;
using LittleTalk.BackendApi.Middleware;
using LittleTalk.Database;
using LittleTalk.Domain;
using LittleTalk.Domain.Security;

var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? "LittleTalk.BackendApi";
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
    Log.Information("[BackendApi] Main => Application starting up.");

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();

    // 1. Feature Controllers
    builder.Services.AddControllers();

    // 2. Register Database & Domain Services
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=.;Database=LittleTalkDb;Trusted_Connection=True;TrustServerCertificate=True;";
    builder.Services.AddDbContext<LittleTalkDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddDomainServices();

    // Outbound HTTP Client for AI Lesson Generation
    builder.Services.AddHttpClient("AiClient", client =>
    {
        client.Timeout = TimeSpan.FromSeconds(60);
    });

    // 3. Configure JWT Authentication
    var jwtSecret = builder.Configuration["JwtSettings:Secret"] 
        ?? JwtTokenService.DefaultSecret;
    var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] 
        ?? JwtTokenService.DefaultIssuer;
    var jwtAudience = builder.Configuration["JwtSettings:Audience"] 
        ?? JwtTokenService.DefaultAudience;

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("TeacherOrAdmin", policy => policy.RequireRole("Teacher", "Admin"));
    });

    // 4. Configure CORS for Blazor Client
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // 5. Swagger with JWT Support
    builder.Services.AddSwaggerWithJwt();

    // 6. Health Checks
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Configure Middleware Pipeline
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        await next();
    });

    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LittleTalk API v1");
        c.RoutePrefix = "swagger";
    });

    // Redirect root to swagger
    app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/health");
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "[BackendApi] Main => Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
