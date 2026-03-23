using Serilog;

namespace CineVault.API.Extensions;

public static class SerilogExtensions
{
    public static void AddLogging(this WebApplicationBuilder build)
    {
        Log.Logger = new LoggerConfiguration()
         .MinimumLevel.Information()
         .Enrich.FromLogContext()
         .Enrich.WithMachineName()
         .Enrich.WithThreadId()
         .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
         .WriteTo.File(          
         path: "Logs/cinevault-zhila-vitalii-.txt",
         rollingInterval: RollingInterval.Day,
         outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
 .CreateLogger();

        build.Host.UseSerilog();
        
    }
}
