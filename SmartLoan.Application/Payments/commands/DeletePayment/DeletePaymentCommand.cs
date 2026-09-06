using MediatR;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Payments.commands.DeletePayment;

public record DeletePaymentCommand(int Id) : IRequest;

public class DeletePaymentHandler(IApplicationDbContext context) : IRequestHandler<DeletePaymentCommand>

{
    public async Task Handle(DeletePaymentCommand request, CancellationToken ct)
    {
        var payment = await context.Payments.FindAsync([request.Id], ct);
        if (payment == null) throw new NotFoundException("Payment not found");

        context.Payments.Remove(payment); // This is a HARD delete because it's a transaction error fix
        await context.SaveChangesAsync(ct);
    }
}