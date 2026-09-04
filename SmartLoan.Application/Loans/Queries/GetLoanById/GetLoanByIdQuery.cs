using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Application.Common; // Important!

namespace SmartLoan.Application.Loans.Queries.GetLoanById;

public record GetLoanByIdQuery(int Id) : IRequest<LoanDto>;

public class GetLoanByIdHandler(IApplicationDbContext context)
    : IRequestHandler<GetLoanByIdQuery, LoanDto>
{
    public async Task<LoanDto> Handle(GetLoanByIdQuery request, CancellationToken cancellationToken)
    {
        var loan = await context.LoanApplications
            .AsNoTracking()
            .Include(l => l.Customer)
            .Include(l=>l.Payments) // Load the name
            .FirstOrDefaultAsync(l => l.Id == request.Id,cancellationToken);

        if (loan == null) throw new NotFoundException($"Loan with ID {request.Id} not found.");

        decimal totalSaved = loan.Payments.Sum(p => p.Amount);
        int daySaved = (int)(totalSaved / loan.DailyAmount);

        var links = new List<LinkDto>
        {
            new($"/api/v1/loans/{loan.Id}", "self", "GET"),
            new($"/api/v1/loans/{loan.Id}/payments", "payments", "GET")
        };
       if(loan.Status=="Saving" && daySaved >= 45)
       {
           links.Add(new($"/api/v1/loans/{loan.Id}/approve", "approve", "POST"));
       }
       if(loan.Status != "Approved")
        {
            links.Add(new ($"/api/v1/loans/{loan.Id}", "delete", "DELETE"));
        }

        return new LoanDto(loan.Id,
            loan.Customer!.FullName,
            loan.DailyAmount, 
            loan.LoanAmount, 
            loan.ServiceFee,
            loan.Status,
            loan.CreatedAt,
            (int)(loan.Payments.Sum(p => p.Amount) / loan.DailyAmount),links);
    }
}