using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Infrastructure.Identity;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public string? Role => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
}