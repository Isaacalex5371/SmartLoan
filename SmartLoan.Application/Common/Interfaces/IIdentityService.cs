namespace SmartLoan.Application.Common.Interfaces;

public interface IIdentityService
{
    string GenerateToken(int userId, string fullName, string email, string role);
    string GenerateRefreshToken();
}