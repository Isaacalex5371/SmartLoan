using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Loans.Queries.GetLoanProgress;

public record GetLoanProgressQuery(int LoanId):IRequest<LoanProgressDto>;

public record LoanProgressDto(
    int LoanId,
    string CustomerName,
    int DaysSaved,
    int DaysRemaining,
    bool IsEligible,
    decimal TotalAmountSaved);
    public class GetLoanProgressHandler(IApplicationDbContext context) : IRequestHandler<GetLoanProgressQuery, LoanProgressDto>
{
    public async Task<LoanProgressDto> Handle(GetLoanProgressQuery request, CancellationToken cancellationToken)
    {
        var loan = await context.LoanApplications
            .Include(l => l.Customer)
            .Include(l => l.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.LoanId, cancellationToken);
        if (loan == null) throw new Exception("Loan Not found");
        int daysSaved = loan.Payments.Count();
        int dayRemaining = 45 - daysSaved;
        if (dayRemaining < 0) dayRemaining = 0;

        return new LoanProgressDto(
            loan.Id,
            loan.Customer!.FullName,
            daysSaved,
            dayRemaining,
            daysSaved >= 45,
            loan.Payments.Sum(p => p.Amount)
        );

    }
}