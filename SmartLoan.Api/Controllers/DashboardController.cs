using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Dashboard;
using SmartLoan.Application.Dashboard.Queries;

namespace SmartLoan.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class DashboardController(IMediator mediator) : ControllerBase 
{
[HttpGet]
public async Task<ActionResult<AdminDashboardDto>> GetStats()
{
    return await mediator.Send(new GetAdminDashboardQuery());
}

[HttpPost("reports")]
public async Task<IActionResult> RequestReport(GenerateReportCommand command)
{
    var reportId = await mediator.Send(command);
    return Accepted($"/api/v1/dashboard/reports/{reportId}", new { id = reportId, status = "Processing" });
}
}