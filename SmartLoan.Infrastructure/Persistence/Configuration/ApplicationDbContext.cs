using Microsoft.EntityFrameworkCore;
using SmartLoan.Domain.Entities;

namespace SmartLoan.Infrastructure.Persistence.Configurations;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly
        );
    }


}