using MediatR;

namespace SmartLoan.Application.Users;

public record RegisterUserCommand
(string FullName, string Email, string Password, string Role) : IRequest<int>;