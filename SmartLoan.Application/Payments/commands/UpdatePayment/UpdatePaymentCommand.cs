using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Exceptions;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Payments.commands.UpdatePayment;

public record UpdatePaymentCommand(int Id, decimal NewAmount) : IRequest;

public class UpdatePaymentHandler(IApplicationDbContext context) : IRequestHandler<UpdatePaymentCommand>
{
    public async Task Handle(UpdatePaymentCommand request, CancellationToken ct)
    {
        var payment = await context.Payments
            .Include(p => p.LoanApplication)
            .ThenInclude(l => l.Payments)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (payment == null) throw new NotFoundException("Payment not found");
        var loan = payment.LoanApplication!;

        // 1. Check Modulo
        if (request.NewAmount % loan.DailyAmount != 0)
            throw new BusinessRuleException($"Amount must be a multiple of {loan.DailyAmount}");

        // 2. Check 105-Day Cap (Excluding the current payment's old amount)
        decimal otherPaymentsTotal = loan.Payments.Where(p => p.Id != request.Id).Sum(p => p.Amount);
        if ((otherPaymentsTotal + request.NewAmount) / loan.DailyAmount > 105)
            throw new BusinessRuleException("Updated amount exceeds the 105-day limit.");

        payment.Amount = request.NewAmount;
        await context.SaveChangesAsync(ct);
    }
}