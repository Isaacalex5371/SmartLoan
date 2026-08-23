using MediatR;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Customers;

public record UpdateCustomerCommand(int Id, string FullName, string Phone, string Address) :IRequest;

public class UpdateCustomerCHandler(IApplicationDbContext context) : IRequestHandler<UpdateCustomerCommand>
{
    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null) throw new Exception("customer not found");

        entity.FullName = request.FullName;
        entity.Phone = request.Phone;
        entity.Address = request.Address;
        await context.SaveChangesAsync(cancellationToken);
        
    }
}