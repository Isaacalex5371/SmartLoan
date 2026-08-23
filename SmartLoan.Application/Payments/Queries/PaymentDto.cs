using SmartLoan.Application.Common;

namespace SmartLoan.Application.Payments;

public record PaymentDto(int Id, decimal Amount, DateTime PaymentDate,List<LinkDto> links);