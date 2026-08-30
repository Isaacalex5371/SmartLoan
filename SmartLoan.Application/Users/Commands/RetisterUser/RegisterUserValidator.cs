using FluentValidation;

namespace SmartLoan.Application.Users.Commands.RegisterUser;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);

        // Ensure they don't try to create a "Hacker" role
        RuleFor(x => x.Role).Must(role => role == "Administrator" || role == "LoanOfficer")
            .WithMessage("Role must be either 'Administrator' or 'LoanOfficer'.");
    }
}