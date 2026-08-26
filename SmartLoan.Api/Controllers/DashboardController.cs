using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Dashboard;
using SmartLoan.Application.Dashboard.Queries;
using SmartLoan.Application.Dashboard.Queries.GetLoanSummary;

namespace SmartLoan.Api.Controllers;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Administrator")]
public class DashboardController(IMediator mediator) : ControllerBase 
{
    [HttpGet]
    public async Task<ActionResult<AdminDashboardDto>> GetStats()
    {
        return await mediator.Send(new GetAdminDashboardQuery());
    }
    [HttpGet("summary")]
    [Authorize(Roles = "Administrator")]
public async Task<ActionResult<LoanSummaryDto>> GetSummary()
    {
        return await mediator.Send(new GetLoansummatyQuery());
    }

    [HttpPost("reports")]
    public async Task<IActionResult> RequestReport(GenerateReportCommand command)
    {
        var reportId = await mediator.Send(command);
        return Accepted($"/api/v1/dashboard/reports/{reportId}", new { id = reportId, status = "Processing" });
    }

}