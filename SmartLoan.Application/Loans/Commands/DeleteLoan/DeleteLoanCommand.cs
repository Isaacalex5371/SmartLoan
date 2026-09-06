using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Exceptions;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Loans;

public record DeleteLoanCommand(int Id) :IRequest;
public class DeleteLoanHandler(IApplicationDbContext context) : IRequestHandler<DeleteLoanCommand>
{
    public async Task Handle(DeleteLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await context.LoanApplications
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (loan == null) throw new NotFoundException("Loan not found");
        if (loan.Status == "Approved")
            throw new BusinessRuleException("cannot delete a loan that has already been approved.");
        loan.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);
    }
}