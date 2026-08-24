namespace SmartLoan.Application.Common.Interfaces;

public interface IReportQueue
{
    // The Application only knows it can "Queue" a report
    ValueTask QueueReportAsync(string reportId);
}