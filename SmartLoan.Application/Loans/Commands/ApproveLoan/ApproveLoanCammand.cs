using MediatR;

namespace SmartLoan.Application.Loans;

public record ApproveLoanCammand(int LoanId) : IRequest<string>;
