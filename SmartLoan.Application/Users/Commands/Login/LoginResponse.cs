using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Exceptions;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Users;

public record LoginResponse(string Token,string RefreshToken, string FullName, string Role);

public record LoginCommand(string Email, string Password):IRequest<LoginResponse>;
public class LoginHandler(IApplicationDbContext context ,IIdentityService identityService): IRequestHandler<LoginCommand,LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await context.Users.FirstOrDefaultAsync(
            u => u.Email == request.Email && u.PasswordHash == request.Password, ct);

        if (user == null) throw new BusinessRuleException("Invalid email or password.");
        var token = identityService.GenerateToken(user.Id, user.FullName, user.Email, user.Role);
        var refreshToken = identityService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync(ct);

        return new LoginResponse(token,refreshToken, user.FullName, user.Role);
    }
}