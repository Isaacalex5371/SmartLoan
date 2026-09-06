using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Customers;

public record CustomerListDto(
    int Id,
    string FullName,
    string Phone,
    string Address);

public record GetCustomersQuery(PagedRequest Request) : IRequest<PagedResponse<CustomerDetailsDto>>;
public class GetCustomersHandler(IApplicationDbContext context)
    : IRequestHandler<GetCustomersQuery, PagedResponse<CustomerDetailsDto>>
{
    public async Task<PagedResponse<CustomerDetailsDto>> Handle(GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Start with IQueryable (No SQL runs yet!)
        var query = context.Customers.AsNoTracking();

        // 2. Filter / Search
        if (!string.IsNullOrWhiteSpace(request.Request.Search))
        {
            var search = request.Request.Search.ToLower();
            query = query.Where(c => c.FullName.ToLower().Contains(search) ||
                                     c.Phone.Contains(search));
        }

        // 3. Count Total (Important: Count BEFORE Skip/Take)
        var totalCount = await query.CountAsync(cancellationToken);

        // 4. Paginate and Project
        var items = await query
            .OrderBy(c => c.FullName) // Always OrderBy before Skip/Take!
            .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .Select(c => new CustomerDetailsDto(c.Id, c.FullName, c.Phone, c.Address,new List<LinkDto>
            {
                new($"/api/v1/customers/{c.Id}", "details", "GET")}))
            .ToListAsync(cancellationToken);

        return new PagedResponse<CustomerDetailsDto>(
            items,
            totalCount,
            request.Request.PageNumber,
            request.Request.PageSize);
    }
}