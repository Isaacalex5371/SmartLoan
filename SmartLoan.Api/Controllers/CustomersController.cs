using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Customers.Commands.CreateCustomer;

namespace SmartLoan.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")] // Versioning starts here!
public class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerCommand command)
    {
        // Messenger just sends the command and waits for the ID
        var customerId = await mediator.Send(command);

        // Returns 201 Created
        return CreatedAtAction(nameof(Create), new { id = customerId }, customerId);
    }
}