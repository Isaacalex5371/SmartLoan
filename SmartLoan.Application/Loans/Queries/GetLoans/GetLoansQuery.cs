using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Application.Loans.Queries;

namespace SmartLoan.Application.Loans.GetLoans;
public record GetLoansQuery(PagedRequest Request) : IRequest<PagedResponse<LoanDto>>;

public class GetLoansHandler(IApplicationDbContext context)
    : IRequestHandler<GetLoansQuery, PagedResponse<LoanDto>>
{
    public async Task<PagedResponse<LoanDto>> Handle(GetLoansQuery request, CancellationToken cancellationToken)
    {
        var query = context.LoanApplications.Include(l => l.Customer).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Request.Search))
        {
            query = query.Where(l => l.Customer!.FullName.Contains(request.Request.Search) ||
                                     l.Status.Contains(request.Request.Search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .Select(l => new LoanDto(
                l.Id,
                l.Customer!.
                    FullName, 
                l.DailyAmount, 
                l.LoanAmount,
                l.ServiceFee,
                l.Status,
                l.CreatedAt,(int)(l.Payments.Sum(p => p.Amount) / l.DailyAmount),
                new List<LinkDto>
                {
                    new(
                        $"/api/v1/loans/{l.Id}", "details", "GET")
                })).ToListAsync(cancellationToken);
          

        return new PagedResponse<LoanDto>(items, totalCount, request.Request.PageNumber, request.Request.PageSize);
    }
}