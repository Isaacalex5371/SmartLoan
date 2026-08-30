using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLoan.Domain.Entities;

namespace SmartLoan.Infrastructure.Persistence.Configuration;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
 public void Configure(EntityTypeBuilder<Customer> builder)
 {
     builder.HasKey(c => c.Id);
     builder.Property(c => c.FullName).IsRequired().HasMaxLength(100);
     builder.Property(c => c.Address).IsRequired().HasMaxLength(250);
     builder.Property(c => c.Address).IsRequired().HasMaxLength(250);
     builder.HasMany(c => c.Applications).WithOne(a => a.Customer).OnDelete(DeleteBehavior.Restrict);
 }
}