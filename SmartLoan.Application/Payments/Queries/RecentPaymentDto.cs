namespace SmartLoan.Application.Payments;

public record RecentPaymentDto
(
    string CustomerName,
    decimal Amount,
    DateTime Date,

int LoanId

);