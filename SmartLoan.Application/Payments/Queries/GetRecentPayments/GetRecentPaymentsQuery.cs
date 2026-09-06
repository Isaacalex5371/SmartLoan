using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Payments.GetRecentPayments;

public record GetRecentPaymentsQuery : IRequest<List<RecentPaymentDto>>;
public class GetRecentPaymentsHandler(IApplicationDbContext context) : IRequestHandler<GetRecentPaymentsQuery, List<RecentPaymentDto>>
{
    public async Task<List<RecentPaymentDto>> Handle(GetRecentPaymentsQuery request, CancellationToken ct)
    {
        return await context.Payments
        .AsNoTracking()
        .Include(p => p.LoanApplication.Customer)
        .OrderByDescending(p => p.PaymentDate)
        .Take(10)
        .Select(p => new RecentPaymentDto(
 p.LoanApplication!.Customer!.FullName,
 p.Amount, p.PaymentDate,
 p.LoanApplicationId
        )).ToListAsync(ct);
    }
}