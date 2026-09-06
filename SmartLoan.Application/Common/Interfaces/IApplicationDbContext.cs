using Microsoft.EntityFrameworkCore;
using SmartLoan.Domain.Entities;

namespace SmartLoan.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<LoanApplication> LoanApplications { get; }
    DbSet<Payment> Payments { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}