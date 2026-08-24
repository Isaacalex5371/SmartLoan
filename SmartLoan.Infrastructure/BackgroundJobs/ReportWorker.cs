using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace SmartLoan.Infrastructure.BackgroundJobs;

public class ReportWorker(ReportQueue queue, ILogger<ReportWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Report Worker is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            // 1. Wait for an item in the queue
            var reportId = await queue.DequeueAsync(stoppingToken);

            logger.LogInformation("Chef picking up Report {Id}. Processing...", reportId);

            // 2. Simulate heavy work (e.g. generating a PDF)
            await Task.Delay(10000, stoppingToken);

            logger.LogInformation("Report {Id} finished! (Ready for download)", reportId);
        }
    }
}