using MediatR;
using SmartLoan.Application.Common.Interfaces;

namespace SmartLoan.Application.Dashboard;

public record GenerateReportCommand(string ReportType) : IRequest<string>;
public class GenerateReportHandler(IReportQueue queue) : IRequestHandler<GenerateReportCommand, string>
{
    public async Task<string> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
    {
        var reportId = Guid.NewGuid().ToString()[..8];
        await queue.QueueReportAsync(reportId);
        return reportId;
    }
}