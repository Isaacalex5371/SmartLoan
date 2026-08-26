using NLog;

namespace SmartLoan.Api.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        using (MappedDiagnosticsContext.SetScoped("CorrelationId", correlationId))
        {
            await next(context);
        }
    }

}