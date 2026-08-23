using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Loans;

public class ApproveLoanHandler(IApplicationDbContext context):IRequestHandler<ApproveLoanCammand,string>
{
    public async Task<string> Handle(ApproveLoanCammand request, CancellationToken cancellationToken)
    {
        var loan = await context.LoanApplications.Include(l => l.Payments)
            .FirstOrDefaultAsync(l => l.Id == request.LoanId, cancellationToken);

        if (loan == null) throw new Exception("Loan not found");

        decimal totalSaved = loan.Payments.Sum(p => p.Amount);
        int daysSaved = (int)(totalSaved / loan.DailyAmount);

        if (daysSaved < 45)
        {
            throw new Exception(
                $"Customer is not eligible. They have only saved for {daysSaved} days. 45 days required.");
        }

        decimal netPayout = loan.LoanAmount - loan.ServiceFee;
        loan.Status = "Approved";

        await context.SaveChangesAsync(cancellationToken);
        return
            $"Loan Approved! Total Loan: {loan.LoanAmount} ETB. Service Fee (5%): {loan.ServiceFee} ETB. Net Payout to Customer: {netPayout} ETB.";
    }
}