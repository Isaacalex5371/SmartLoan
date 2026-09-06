using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Payments;
using SmartLoan.Application.Payments.commands.DeletePayment;
using SmartLoan.Application.Payments.commands.UpdatePayment;
using SmartLoan.Application.Payments.GetRecentPayments;

namespace SmartLoan.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PaymentsController(IMediator mediator):ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Record(RecordPaymentCommand command)
    {
        var paymentId = await mediator.Send(command);
        return Ok(paymentId);
    }
    [HttpGet("recent")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<List<RecentPaymentDto>>> GetRecent()
    {
        return await mediator.Send(new GetRecentPaymentsQuery());
    }
[HttpPut("{id}")]
[Authorize(Roles = "Administrator")]
public async Task<IActionResult> Update(int id, UpdatePaymentCommand command)
{
    if (id != command.Id) return BadRequest();
    await mediator.Send(command);
    return NoContent();
}

[HttpDelete("{id}")]
[Authorize(Roles = "Administrator")]
public async Task<IActionResult> Delete(int id)
{
    await mediator.Send(new DeletePaymentCommand(id));
    return NoContent();
}
}