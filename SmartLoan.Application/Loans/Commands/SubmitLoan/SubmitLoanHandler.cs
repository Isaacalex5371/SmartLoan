using MediatR;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Domain.Entities;

namespace SmartLoan.Application.Loans.Commands.SubmitLoan;

public class SubmitLoanHandler(IApplicationDbContext context)
    : IRequestHandler<SubmitLoanCommand, int>
{
    public async Task<int> Handle(SubmitLoanCommand request, CancellationToken cancellationToken)
    {
        // Do the math based on your rule
        var loanAmount = request.DailyAmount * 100;
        var serviceFee = loanAmount * 0.05m;

        var loan = new LoanApplication
        {
            CustomerId = request.CustomerId,
            DailyAmount = request.DailyAmount, // Use this
            LoanAmount = loanAmount, // Use this
            ServiceFee = serviceFee, // Use this
            Status = "Saving"
        };

        context.LoanApplications.Add(loan);
        await context.SaveChangesAsync(cancellationToken);

        return loan.Id;
    }
}