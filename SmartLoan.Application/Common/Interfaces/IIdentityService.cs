namespace SmartLoan.Application.Common.Interfaces;

public interface IIdentityService
{
    string GenerateToken(int userId, string fullName, string email, string role);
    string GenerateRefreshToken();
    public string HashPassword(string password);
    public bool VerifyPassword(string password, string hash);
}