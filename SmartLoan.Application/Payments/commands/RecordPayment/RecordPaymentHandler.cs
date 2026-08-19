using MediatR;
using Microsoft.EntityFrameworkCore;
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
