using System.Diagnostics;

namespace CineVault.API;

public class PerformanceLoggingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<PerformanceLoggingMiddleware> logger;
    public PerformanceLoggingMiddleware(RequestDelegate next,
   ILogger<PerformanceLoggingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await this.next(context);
        }
        finally
        {
            stopwatch.Stop();
            var path = context.Request.Path.Value ?? "";
            var method = context.Request.Method;
            var duration = stopwatch.ElapsedMilliseconds;
            this.logger.LogInformation(
            "HTTP {Method} {Path} responded {StatusCode} in {Duration}ms",
            method, path, context.Response.StatusCode, duration);
            // Додаткове логування повільних запитів
            if (duration > 500)
            {
                this.logger.LogWarning("Slow request detected: {Method} {Path}took { Duration}ms", method, path, duration);
            }
        }
    }
}