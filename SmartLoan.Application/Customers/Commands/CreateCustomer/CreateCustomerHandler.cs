using MediatR;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Domain.Entities;
namespace SmartLoan.Application.Customers.Commands.CreateCustomer;

// Notice the Primary Constructor injecting our DbContext
public class CreateCustomerHandler(IApplicationDbContext context)
    : IRequestHandler<CreateCustomerCommand, int>
{
    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // 1. Create the Domain Entity
        var customer = new Customer
        {
            FullName = request.FullName,
            Phone = request.Phone,
            Address = request.Address
        };

        // 2. Add to the Change Tracker
        context.Customers.Add(customer);

        // 3. Save to PostgreSQL
        await context.SaveChangesAsync(cancellationToken);

        // 4. Return the new ID
        return customer.Id;
    }
}