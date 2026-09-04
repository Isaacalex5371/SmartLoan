using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Application.Loans.Queries;

namespace SmartLoan.Application.Customers;

public record GetCustomerLoansQuery(int CustomerId) : IRequest<List<LoanDto>>;
public class GetCustomerLoanHandler(IApplicationDbContext context) : IRequestHandler<GetCustomerLoansQuery, List<LoanDto>>
{
    
    public async Task<List<LoanDto>> Handle(GetCustomerLoansQuery request, CancellationToken ct)
    {
        return await context.LoanApplications.AsNoTracking()
        .Where(l => l.CustomerId == request.CustomerId)
        .OrderByDescending(l => l.CreatedAt)
        .Select(l => new LoanDto(
l.Id,
l.Customer!.FullName,
l.DailyAmount,
 l.LoanAmount,
  l.ServiceFee,
  l.Status,
  l.CreatedAt,(int)(l.Payments.Sum(p => p.Amount) / l.DailyAmount),
   new List<LinkDto> { new($"/api/v1/loans/{l.Id}", "self", "GET") }
        )).ToListAsync(ct);
    }

}