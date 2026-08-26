using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Common;
using SmartLoan.Application.Loans;
using SmartLoan.Application.Loans.Commands.SubmitLoan;
using SmartLoan.Application.Loans.GetLoans;
using SmartLoan.Application.Loans.Queries;
using SmartLoan.Application.Loans.Queries.GetLoanById;
using SmartLoan.Application.Loans.Queries.GetLoanProgress;
using SmartLoan.Application.Payments;
using SmartLoan.Application.Payments.GetPaymentHistory;

namespace SmartLoan.Api.Controllers;

[ApiController]

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class LoanController(IMediator mediator): ControllerBase
{
    [HttpPost ]
    public async Task<IActionResult> Submit(SubmitLoanCommand command)
    {
        var loanId = await mediator.Send(command);
        return CreatedAtAction(nameof(Submit), new { id = loanId }, loanId);
    }

[HttpGet]
public async Task<ActionResult<PagedResponse<LoanDto>>> GetAll([FromQuery] PagedRequest request)
{
    return await mediator.Send(new GetLoansQuery(request));
}

[HttpGet("{id}/progress")]
public async Task<ActionResult<LoanProgressDto>> GetProgress(int id)
{
    return await mediator.Send(new GetLoanProgressQuery(id));
}

[HttpPost ("{id}/approve")]
[Authorize(Roles = "Administrator")]
public async Task<IActionResult> Approve(int id)
{
    var result = await mediator.Send(new ApproveLoanCammand(id));
    return Ok(new { message = result });
}


[HttpGet("{id}")]
public async Task<ActionResult<LoanDto>> GetById(int id)
{
    return await mediator.Send(new GetLoanByIdQuery(id));
}
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    await mediator.Send(new DeleteLoanCommand(id));
    return NoContent();
}
[HttpGet("{id}/payments")]
public async Task<ActionResult<List<PaymentDto>>> GetPaymentS(int id)
{
    return await mediator.Send(new GetPaymentHistoryQuery(id));
} 
}