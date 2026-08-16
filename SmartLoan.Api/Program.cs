using Microsoft.EntityFrameworkCore;
using SmartLoan.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();

app.Run();