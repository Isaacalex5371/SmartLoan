using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Loans.Commands.SubmitLoan;
using SmartLoan.Application.Loans.GetLoans;
using SmartLoan.Application.Loans.Queries;
using SmartLoan.Application.Loans.Queries.GetLoanProgress;

namespace SmartLoan.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LoanController(IMediator mediator): ControllerBase
{
    [HttpPost ]
    public async Task<IActionResult> Submit(SubmitLoanCommand command)
    {
        var loanId = await mediator.Send(command);
        return CreatedAtAction(nameof(Submit), new { id = loanId }, loanId);
    }

[HttpGet]
public async Task<ActionResult<List<LoanDto>>> GetAll()
{
    return await mediator.Send(new GetLoanQuery());
}

[HttpGet("{id}/progress")]
public async Task<ActionResult<LoanProgressDto>> GetProgress(int id)
{
    return await mediator.Send(new GetLoanProgressQuery(id));
}


}