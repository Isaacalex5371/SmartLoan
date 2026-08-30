namespace SmartLoan.Api.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next,IWebHostEnvironment env)
{ 
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/scalar") ||
            context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/openapi"))
        {
            await next(context);
            return;
        }

        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        if (!env.IsDevelopment())
        {
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';");
        }

        await next(context);
    }

}