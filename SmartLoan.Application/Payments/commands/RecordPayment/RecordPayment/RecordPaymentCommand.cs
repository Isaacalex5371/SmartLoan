using MediatR;

namespace SmartLoan.Application.Payments;

public record RecordPaymentCommand(
    int LoanApplicationId,
    decimal Amount) : IRequest<int>;
