using SmartLoan.Application.Common;

namespace SmartLoan.Application.Loans.Queries;

public record LoanDto(
    int Id,
    string CustomerName,
    decimal DailyAmount, // Added
    decimal LoanAmount, // Renamed
    decimal ServiceFee, // Added
    string Status,
    DateTime CreatedAt,
    List<LinkDto> Links);