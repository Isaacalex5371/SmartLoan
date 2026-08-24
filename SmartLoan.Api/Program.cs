using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SmartLoan.Api.Middleware;
using SmartLoan.Application.Common.Behaviors;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Infrastructure.BackgroundJobs;
using SmartLoan.Infrastructure.Notifications;
using SmartLoan.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidatorsFromAssembly(typeof(SmartLoan.Application.Common.Interfaces.IApplicationDbContext)
    .Assembly);
builder.Services.AddSingleton<ReportQueue>();
builder.Services.AddSingleton<IReportQueue>(provider => provider.GetRequiredService<ReportQueue>());
builder.Services.AddHostedService<ReportWorker>();

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
app.UseCors("SignalRPolicy");
app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if  (app.Environment.IsDevelopment())

{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.MapHub<NotificationHub>("/notifications");
app.UseHttpsRedirection();

app.MapControllers();

app.Run();