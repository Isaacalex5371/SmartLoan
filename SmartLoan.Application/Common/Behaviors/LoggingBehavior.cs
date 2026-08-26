using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SmartLoan.Application.Common.Behaviors;

public class LoggingBehavior<TRequest,TResponse>(ILogger<LoggingBehavior<TRequest,TResponse>> logger):IPipelineBehavior<TRequest,TResponse> where TRequest: notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var timer = Stopwatch.StartNew();
        logger.LogInformation("smartloan Request:{Name} starting...",requestName);
        var response = await next();
        timer.Stop();
        logger.LogInformation("Smarloan Request:{Name} finished in {Elapsed}ms",requestName,timer.ElapsedMilliseconds);
        return response;
    }
}