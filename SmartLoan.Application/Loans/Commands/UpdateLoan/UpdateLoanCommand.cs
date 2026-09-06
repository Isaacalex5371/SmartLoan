using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Application.Common.Exceptions;
using SmartLoan.Application.Common;

namespace SmartLoan.Application.Loans.Commands.UpdateLoan;

public record UpdateLoanCommand(int Id, decimal DailyAmount) : IRequest;

public class UpdateLoanHandler(IApplicationDbContext context) : IRequestHandler<UpdateLoanCommand>
{
    public async Task Handle(UpdateLoanCommand request, CancellationToken ct)
    {
        var loan = await context.LoanApplications
            .Include(l => l.Payments)
            .FirstOrDefaultAsync(l => l.Id == request.Id, ct);

        if (loan == null) throw new NotFoundException("Loan not found");

        // BUSINESS RULE: If they already started saving, don't allow changing the rate
        if (loan.Payments.Any())
            throw new BusinessRuleException("Cannot change Daily Amount because payments have already been recorded.");

        // Update the values and recalculate the dependent totals
        loan.DailyAmount = request.DailyAmount;
        loan.LoanAmount = request.DailyAmount * 100;
        loan.ServiceFee = loan.LoanAmount * 0.05m;

        await context.SaveChangesAsync(ct);
    }
}