using MediatR;

namespace SmartLoan.Application.Loans.Commands.SubmitLoan;

// Change 'decimal Amount' to 'decimal DailyAmount'
// We remove Duration because it's always 105 days now
public record SubmitLoanCommand(
    int CustomerId,
    decimal DailyAmount) : IRequest<int>;