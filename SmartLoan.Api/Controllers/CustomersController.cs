using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Common;
using SmartLoan.Application.Customers;
using SmartLoan.Application.Customers.Commands.CreateCustomer;
using SmartLoan.Application.Loans.Queries;

namespace SmartLoan.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]

[Route("api/v{version:apiVersion}/[controller]")] // Versioning starts here!
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
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CustomerDetailsDto>>> GetAll([FromQuery] PagedRequest request)
    {
        return await mediator.Send(new GetCustomersQuery(request));
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDetailsDto>> Get(int id)
    {
        var customer = await mediator.Send(new GetCustomerByIdQuery(id));
        if (customer == null)
        {
            return NotFound(new
            {
                message = "customer not found"
            });
        }

        return Ok(customer);
    }
    [HttpGet("{id}/loans")]
    public async Task<ActionResult<List<LoanDto>>> GetCustomerLoans(int id)
    {
        return await mediator.Send(new GetCustomerLoansQuery(id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,UpdateCustomerCommand command)
    {
        if (id != command.Id) return BadRequest();
        await mediator.Send(command);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteCustomerCommand(id));
        return NoContent();
    }
}