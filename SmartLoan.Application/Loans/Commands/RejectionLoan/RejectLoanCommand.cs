using MediatR;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Exceptions;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Loans;

public record RejectLoanCommand(int LoanId, string Reason) : IRequest;
public class RejectLoanHandler(IApplicationDbContext context): IRequestHandler<RejectLoanCommand>
{
  
public async Task Handle(RejectLoanCommand request, CancellationToken ct)
    {
        var loan = await context.LoanApplications.FindAsync([request.LoanId], ct);
        if (loan == null) throw new NotFoundException("Loan not found");
        if (loan.Status != "Saving") throw new BusinessRuleException("only loans in 'Saving' status can be rejected.");
        loan.Status = "Rejected";
        await context.SaveChangesAsync(ct);

    }
}