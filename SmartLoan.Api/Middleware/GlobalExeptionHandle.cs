using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartLoan.Application.Common;
using SmartLoan.Application.Common.Exceptions;

namespace SmartLoan.Api.Middleware;

public class GlobalExeptionHandle(ILogger<GlobalExeptionHandle> logger):IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
       logger.LogError(exception,"an error occurred:{Message}",exception.Message);
       var (statusCode, title) = exception switch
       {
           NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
           BusinessRuleException => (StatusCodes.Status500InternalServerError, "Server Error")
       };
       var problemDetails = new ProblemDetails
       {
           Status = statusCode,
           Title = title,
           Detail = exception.Message,
           Instance = $"{httpContext.Request.Method}{httpContext.Request.Path}"
       };
       httpContext.Response.StatusCode = statusCode;
       await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
       return true;
    }
}