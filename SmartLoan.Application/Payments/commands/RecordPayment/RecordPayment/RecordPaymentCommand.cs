using MediatR;

namespace SmartLoan.Application.Payments;

public record RecordPaymentCommand(
    int LoanAplicationId,
    decimal Amount) : IRequest<int>;
