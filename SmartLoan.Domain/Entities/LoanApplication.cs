namespace SmartLoan.Domain.Entities;

public class LoanApplication
{
    public int Id { get; init; }
    public int CustomerId { get; set; }

    // The specific rules for your company
    public decimal DailyAmount { get; set; } // e.g. 1000 ETB
    public decimal LoanAmount { get; set; } // DailyAmount * 100
    public decimal ServiceFee { get; set; } // LoanAmount * 0.05

    public int TotalDays { get; init; } = 105; // Constant
    public int EligibilityDays { get; init; } = 45; // Constant

    public string Status { get; set; } = "Saving"; // Starts as "Saving"
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}