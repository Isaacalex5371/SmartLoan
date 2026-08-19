
using MediatR;

namespace SmartLoan.Application.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(
    string FullName,
    string Phone,
    string Address) : IRequest<int>;