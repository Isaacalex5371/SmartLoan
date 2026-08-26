using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Payments;

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

}