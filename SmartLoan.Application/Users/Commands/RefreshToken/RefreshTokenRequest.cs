using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartLoan.Application.Common.Exceptions;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Users;
public record RefreshTokenRequest(string Token, string RefreshToken) : IRequest<LoginResponse>;

public class RefreshTokenHandler(IApplicationDbContext context, IIdentityService identityService)
    : IRequestHandler<RefreshTokenRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(RefreshTokenRequest request, CancellationToken ct)
    {
        // 1. Find the user with this refresh token
        var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, ct);

        // 2. Security Check: Does the token match and is it still valid?
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new BusinessRuleException("Session expired. Please login again.");

        // 3. Generate NEW pair (Rotation)
        var newToken = identityService.GenerateToken(user.Id, user.FullName, user.Email, user.Role);
        var newRefreshToken = identityService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await context.SaveChangesAsync(ct);

        return new LoginResponse(newToken, newRefreshToken, user.FullName, user.Role);
    }
}