using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Customers;


public record CustomerDetailsDto(
    int Id,
    string FullName,
    string Phone,
    string Address,
    List<LinkDto> LinkDtos
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


        var customer = await context.Customers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer == null) throw new NotFoundException("customer not found ");
        var links = new List<LinkDto>
        {
            new($"/api/v1/customers/{customer.Id}", "self", "GET"),
            new($"/api/v1/customers/{customer.Id}", "update", "PUT"),
            new($"/api/v1/customers/{customer.Id}", "delete", "DELETE"),
            // Smart link: Link to the Loans controller to start a new application for THIS customer
            new($"/api/v1/loans?customerId={customer.Id}", "create-loan", "POST")
        };

        return new CustomerDetailsDto(customer.Id, customer.FullName, customer.Phone, customer.Address, links);
    }
}


