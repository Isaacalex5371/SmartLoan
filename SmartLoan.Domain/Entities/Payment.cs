namespace SmartLoan.Domain.Entities;

public class Payment
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public required decimal Amount { get; set; }
    public DateTime PaymentDate { get; init; } = DateTime.UtcNow;

    // Navigation Property
    public LoanApplication? LoanApplication { get; set; }
}