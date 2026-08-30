using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartLoan.Application.Users;
using SmartLoan.Application.Users.Commands.RefreshToken;

namespace SmartLoan.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IMediator mediator): ControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting("login-policy")]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(RefreshTokenRequest command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    [HttpPost("register")]
    [Authorize(Roles = "Administrator")] 
    public async Task<ActionResult<int>> Register(RegisterUserCommand command)
    {
        var userId = await mediator.Send(command);
        return Ok(userId);
    }
}