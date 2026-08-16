namespace SmartLoan.Domain.Entities;

public class LoanApplication
{
    public int Id { get; init; }
    public int CustomerId { get; set; }
    public required decimal Amount { get; set; }
    public required int DurationInMonths { get; set; }
    public required string Status { get; set; } // e.g., "Pending", "Approved", "Rejected"
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    // Navigation Properties
    public Customer? Customer { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}