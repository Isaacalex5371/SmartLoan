using System.Text;
using System.Text.Json;
using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
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


// 2. NLOG CONFIGURATION

LogManager.Setup()
    .LoadConfigurationFromFile("NLog.config");


// ============================================================
// 3. CREATE APPLICATION BUILDER
// ============================================================
var builder = WebApplication.CreateBuilder(args);


// 4. LOGGING
// ============================================================
builder.Logging.ClearProviders();
builder.Host.UseNLog();


// 5. HTTP CONTEXT & CURRENT USER
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


// 6. DATABASE & DATABASE INITIALIZER
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddScoped<DbInitializer>();


// 7. API VERSIONING
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;

        options.ApiVersionReader =
            new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });


// 8. VALIDATION
builder.Services.AddValidatorsFromAssembly(
    typeof(IApplicationDbContext).Assembly);

// 9. MEDIATR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(IApplicationDbContext).Assembly);

    cfg.AddBehavior(
        typeof(IPipelineBehavior<,>),
        typeof(LoggingBehavior<,>));

    cfg.AddBehavior(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>));
});


// 10. IDENTITY & JWT AUTHENTICATION
builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!)),

                ClockSkew = TimeSpan.Zero
            };
    });


// 11. AUTHORIZATION
builder.Services.AddAuthorization();


// 12. RATE LIMITING
builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter(
        "login-policy",
        opt =>
        {
            opt.TokenLimit = 5;
            opt.ReplenishmentPeriod =
                TimeSpan.FromMinutes(1);

            opt.TokensPerPeriod = 2;
            opt.QueueLimit = 0;
        });

    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;
});


// 13. BACKGROUND JOBS & REPORT QUEUE
builder.Services.AddSingleton<ReportQueue>();

builder.Services.AddSingleton<IReportQueue>(provider =>
    provider.GetRequiredService<ReportQueue>());

builder.Services.AddHostedService<ReportWorker>();


// 14. SIGNALR
builder.Services.AddSignalR();


// 15. ERROR HANDLING

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExeptionHandle>();


// 16. CONTROLLERS

builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });

// 17. OPENAPI / SCALAR

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??=
            new OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",

                Name = "Authorization",
                In = ParameterLocation.Header,

                Description =
                    "Enter your JWT token"
            };

        return Task.CompletedTask;
    });
});


// 18. CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularPolicy",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});



// 19. HYBRID CACHE


#pragma warning disable EXTEXP0018

builder.Services.AddHybridCache();

#pragma warning restore EXTEXP0018



// 20. BUILD APPLICATION

var app = builder.Build();



// 21. DATABASE SEEDING

using (var scope = app.Services.CreateScope())
{
    var seeder =
        scope.ServiceProvider.GetRequiredService<DbInitializer>();

    await seeder.SeedAsync();
}



// 22. MIDDLEWARE PIPELINE

app.UseExceptionHandler();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AngularPolicy");

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();



// 23. DEVELOPMENT TOOLS

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Smart Loan API")
            .WithTheme(
                ScalarTheme.BluePlanet);
    });
}



// 24. SIGNALR HUBS

app.MapHub<NotificationHub>("/notifications");



// 25. API CONTROLLERS

app.MapControllers();



// 26. RUN APPLICATION

app.Run();