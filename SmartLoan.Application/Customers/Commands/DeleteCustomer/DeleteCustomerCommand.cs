using MediatR;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Customers;

public record DeleteCustomerCommand (int Id):IRequest;

public class DeleteCustomerHandler(IApplicationDbContext context) : IRequestHandler<DeleteCustomerCommand>
{
    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Customers.FindAsync(new object[]
        {
            request.Id

        }, cancellationToken);
        if (entity == null) throw new NotFoundException($"Customer with ID {request.Id} was not found. ");

  entity.IsDeleted = true;
  await context.SaveChangesAsync(cancellationToken);

  
    }
}