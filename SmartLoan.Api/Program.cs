using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

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
var app = builder.Build();
// Configure the HTTP request pipeline.
if  (app.Environment.IsDevelopment())

{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();

app.MapControllers();

app.Run();