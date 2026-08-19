using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Payments;

namespace SmartLoan.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaymentsController(IMediator mediator):ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Record(RecordPaymentCommand command)
    {
        var paymentId = await mediator.Send(command);
        return Ok(paymentId);
    }

}