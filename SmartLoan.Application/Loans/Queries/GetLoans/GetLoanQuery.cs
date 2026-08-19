using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Application.Loans.Queries;

namespace SmartLoan.Application.Loans.GetLoans;

public record GetLoanQuery : IRequest<List<LoanDto>>;

public class GetLoansHandler(IApplicationDbContext context) : IRequestHandler<GetLoanQuery, List<LoanDto>>
{
    public async Task<List<LoanDto>> Handle(GetLoanQuery request, CancellationToken cancellationToken)
    {
        return await context.LoanApplications
            .AsNoTracking()
            .Select(l => new LoanDto(
                l.Id,
                l.Customer!.FullName,
                l.DailyAmount, // Map this
                l.LoanAmount, // Map this
                l.ServiceFee, // Map this
                l.Status,
                l.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
