using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Customers;


public record CustomerDetailsDto(
    int Id,
    string FullName,
    string Phone,
    string Address
);

public record GetCustomerByIdQuery(int Id)
    : IRequest<CustomerDetailsDto?>;

public class GetCustomerByIdHandler(IApplicationDbContext context)
    : IRequestHandler<GetCustomerByIdQuery, CustomerDetailsDto?>
{
    public async Task<CustomerDetailsDto?> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Customers
            .AsNoTracking()
            .Where(c => c.Id == request.Id && !c.IsDeleted)
            .Select(c => new CustomerDetailsDto(
                c.Id,
                c.FullName,
                c.Phone,
                c.Address
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}


