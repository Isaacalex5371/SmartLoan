using FluentValidation;

namespace SmartLoan.Application.Payments;

public class RecordPaymentValidator : AbstractValidator<RecordPaymentCommand>
{
public RecordPaymentValidator ()
{
    RuleFor(v => v.Amount).GreaterThan(0).WithMessage("Payment must be greater than 0.");
    RuleFor(v => v.LoanAplicationId).NotEmpty().WithMessage("Loan ID is Required.");
}
}