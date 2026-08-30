using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Dashboard.Queries.GetLoanSummary;

public record LoanSummaryDto(decimal TotalMoneyCollected, decimal TotalProjectdProfit, int ActiveSavingLoans, int TotalApprovedLoans);
public record GetLoansummatyQuery : IRequest<LoanSummaryDto>;
public class GetLoanSummaryHandler(IApplicationDbContext context) : IRequestHandler<GetLoansummatyQuery, LoanSummaryDto>
{
    public async Task<LoanSummaryDto> Handle(GetLoansummatyQuery request, CancellationToken ct)
    {
        var moneyCollected = await context.Payments.SumAsync(p => p.Amount, ct);
        var profit = await context.LoanApplications.Where(l => l.Status == "Approved").SumAsync(l => l.ServiceFee, ct);
        var activeSaving = await context.LoanApplications.CountAsync(l => l.Status == "Saving", ct);
        var totalApproved = await context.LoanApplications.CountAsync(l => l.Status == "Approved", ct);
        return new LoanSummaryDto(moneyCollected, profit, activeSaving, totalApproved);

    }
}