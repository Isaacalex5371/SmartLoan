using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Exceptions;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Users.Commands.RefreshToken;
public record RefreshTokenRequest(string Token, string RefreshToken) : IRequest<LoginResponse>;

public class RefreshTokenHandler(IApplicationDbContext context, IIdentityService identityService)
    : IRequestHandler<RefreshTokenRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(RefreshTokenRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new BusinessRuleException("Refresh token is missing from the request.");
        }
        
        var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, ct);

       
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new BusinessRuleException("Session expired. Please login again.");

       
        var newToken = identityService.GenerateToken(user.Id, user.FullName, user.Email, user.Role);
        var newRefreshToken = identityService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await context.SaveChangesAsync(ct);

        return new LoginResponse(newToken, newRefreshToken, user.FullName, user.Role);
    }
}