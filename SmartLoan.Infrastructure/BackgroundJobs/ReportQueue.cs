using System.Threading.Channels;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Infrastructure.BackgroundJobs;

public class ReportQueue:IReportQueue
{
    private readonly Channel<string> _queue = Channel.CreateUnbounded<String>();
    public ValueTask QueueReportAsync(string reportId) => _queue.Writer.WriteAsync(reportId);
    public ValueTask<string> DequeueAsync(CancellationToken ct) => _queue.Reader.ReadAsync(ct);
}