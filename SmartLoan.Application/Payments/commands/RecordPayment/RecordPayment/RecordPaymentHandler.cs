using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Exceptions;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Domain.Entities;

namespace SmartLoan.Application.Payments;

public class RecordPaymentHandler(IApplicationDbContext context) :IRequestHandler<RecordPaymentCommand,int>
{
    public async Task<int> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        var loan = await context.LoanApplications.FirstOrDefaultAsync(l =>
            l.Id == request.LoanAplicationId, cancellationToken);
        if (loan == null)
        {
            throw new Exception("loan application not found.");
        }

        
        if(request.Amount % loan.DailyAmount != 0)
        {
            throw new BusinessRuleException($"Invalid amount. for this loan, you must pay in multiples of {loan.DailyAmount}Etb.");
        }

        decimal totalAlreadyPaid = loan.Payments.Sum(p => p.Amount);
        int daysAlreadySaved = (int)(totalAlreadyPaid / loan.DailyAmount);
        int daysCoveredByThisPayment = (int)(request.Amount / loan.DailyAmount);
        if(daysAlreadySaved+ daysCoveredByThisPayment> 105)
        {
            int maxAllowedDays = 105 - daysAlreadySaved;
            decimal maxAllowedAmount = maxAllowedDays * loan.DailyAmount;

            throw new Exception(
                $"Payment exceeds the 105-day limit. You can only pay for {maxAllowedDays} more days ({maxAllowedAmount} ETB).");
        }


        var payment = new Payment
            {
                LoanApplicationId = request.LoanAplicationId,
                Amount =  request.Amount,
                PaymentDate = DateTime.UtcNow
            };
        context.Payments.Add(payment);
        await context.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
    }
