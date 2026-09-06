using System.Security.Claims;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartLoan.Application.Common.Interfaces;
using SmartLoan.Application.Users;
using SmartLoan.Application.Users.Commands.RefreshToken;

namespace SmartLoan.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IMediator mediator, IAntiforgery antiforgery,IIdentityService identityService) : ControllerBase
{
    
    [AllowAnonymous]
    [EnableRateLimiting("login-policy")]
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginCommand command)
    {
        var result = await mediator.Send(command);

        // Is development? (Check if we are on localhost)
        var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            // If we are in Dev, we can use HTTP. In Prod, MUST be HTTPS.
            Secure = !isDev,
            // Lax is better for local dev with different ports (4200 vs 5176)
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(8)
        };

        Response.Cookies.Append("X-Access-Token", result.Token, cookieOptions);

        // Use a slightly different expiration for the refresh token
        var refreshOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDev,
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };


        Response.Cookies.Append("X-Refresh-Token", result.RefreshToken, refreshOptions);
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
        {
            HttpOnly = false,
            Secure = !isDev,
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(8)
        });
        return Ok(new { result.FullName, result.Role });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(RefreshTokenRequest command)
    {
        var result = await mediator.Send(command);

        var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        // Set the new Access Token Cookie
        Response.Cookies.Append("X-Access-Token", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDev,
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(8)
        });

        // Set the new Refresh Token Cookie (Rotation)
        Response.Cookies.Append("X-Refresh-Token", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDev,
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return Ok(new { result.FullName, result.Role });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<int>> Register(RegisterUserCommand command)
    {
        var userId = await mediator.Send(command);
        return Ok(userId);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(
            new
            {
                FullName = User.Identity?.Name,
                Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
            });
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Must match your Login settings
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(-1) // Set to past to force kill
        };

        
        Response.Cookies.Delete("X-Access-Token", cookieOptions);
        Response.Cookies.Delete("X-Refresh-Token", cookieOptions);


        Response.Cookies.Delete("XSRF-TOKEN");

        
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userIdClaim))
        {

            await identityService.RevokeRefreshToken(int.Parse(userIdClaim));
        }


        return NoContent();
    }
}