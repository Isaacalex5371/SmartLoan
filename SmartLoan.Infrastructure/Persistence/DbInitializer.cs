using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SmartLoan.Infrastructure.Persistence;

public class DbInitializer(ApplicationDbContext context, IIdentityService identityService)
{
    public async Task SeedAsync()
    {

        await context.Database.MigrateAsync();

    
        if (!await context.Users.AnyAsync(u => u.Email == "admin@loan.com"))
        {
         
            var admin = new User
            {
                FullName = "System Admin",
                Email = "admin@loan.com",
                Role = "Administrator",
                PasswordHash = identityService.HashPassword("Admin@123") 
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync(default);
        }
    }
}