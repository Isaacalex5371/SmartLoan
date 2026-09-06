using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Payments.GetPaymentHistory;


public record GetPaymentHistoryQuery
(int LoanId): IRequest<List<PaymentDto>>;

public class GetPaymentHistoryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPaymentHistoryQuery, List<PaymentDto>>
{
    public async Task<List<PaymentDto>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        return await context.Payments
            .AsNoTracking()
            .Where(p => p.LoanApplicationId == request.LoanId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentDto(p.Id, p.Amount, p.PaymentDate,
                new List<LinkDto> { new($"/api/v1/loans/{p.LoanApplicationId}", "parent-loan", "GET") }))
            .ToListAsync(cancellationToken);
    }
}