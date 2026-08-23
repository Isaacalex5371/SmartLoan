using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Domain.Entities;

namespace SmartLoan.Infrastructure.Persistence;

// Notice the Primary Constructor (C# 14/10 feature)
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options),IApplicationDbContext
{
    // These DbSets are your Tables
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // This line tells EF to look in this project and find 
        // all our Configuration files automatically!
        modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}