namespace SmartLoan.Domain.Entities;

public class User
{
    public int Id { get; init; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; } // "Administrator" or "LoanOfficer"
}