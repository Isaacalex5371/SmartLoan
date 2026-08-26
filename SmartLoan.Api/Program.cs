using System.Text;
using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Scalar.AspNetCore;
using SmartLoan.Api.Middleware;
using SmartLoan.Application.Common.Behaviors;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Infrastructure.BackgroundJobs;
using SmartLoan.Infrastructure.Identity;
using SmartLoan.Infrastructure.Notifications;
using SmartLoan.Infrastructure.Persistence;
LogManager.Setup().LoadConfigurationFromFile("NLog.config");
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();

}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddValidatorsFromAssembly(typeof(SmartLoan.Application.Common.Interfaces.IApplicationDbContext)
    .Assembly);
builder.Services.AddSingleton<ReportQueue>();
builder.Services.AddSingleton<IReportQueue>(provider => provider.GetRequiredService<ReportQueue>());
builder.Services.AddHostedService<ReportWorker>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))

    };
});

builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("login-policy", opt =>
    {
        opt.TokenLimit = 5;
        opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
        opt.TokensPerPeriod = 2;
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(IApplicationDbContext).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddAuthorization();


builder.Services.AddSignalR();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExeptionHandle>();
// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(
        typeof(SmartLoan.Application.Customers.Commands.CreateCustomer.CreateCustomerCommand).Assembly));
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(IApplicationDbContext).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5176", "http://localhost:5176").AllowAnyHeader().AllowAnyMethod()
            .AllowCredentials();
    });
});

#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache();
var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("SignalRPolicy");
app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if  (app.Environment.IsDevelopment())

{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Smart Loan API").WithTheme(ScalarTheme.BluePlanet);
    });
}
app.MapHub<NotificationHub>("/notifications");
app.UseHttpsRedirection();

app.MapControllers();

app.Run();