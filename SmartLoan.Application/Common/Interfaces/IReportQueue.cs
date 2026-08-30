namespace SmartLoan.Application.Common.Interfaces;

public interface IReportQueue
{

    ValueTask QueueReportAsync(string reportId);
}