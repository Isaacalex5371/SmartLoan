using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Customers;

public record CustomerListDto(
    int Id,
    string FullName,
    string Phone,
    string Address
);

public record GetAllCustomersQuery : IRequest<List<CustomerListDto>>;

public class GetAllCustomersHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllCustomersQuery, List<CustomerListDto>>
{
    public async Task<List<CustomerListDto>> Handle(
        GetAllCustomersQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Customers
            .AsNoTracking()
            .Select(c => new CustomerListDto(
                c.Id,
                c.FullName,
                c.Phone,
                c.Address
            ))
            .ToListAsync(cancellationToken);
    }
}