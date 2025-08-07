using System.Net;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace CoreAPI.Extensions;

public static class LoggingExtensions
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        string? appName = Assembly.GetExecutingAssembly().GetName().Name;
        string sysFromEmail =
            Environment.GetEnvironmentVariable("SYSTEM_FROM_EMAIL")
            ?? throw new InvalidOperationException(
                "SYSTEM_FROM_EMAIL environment variable is not set."
            );
        string? sysToEmail =
            Environment.GetEnvironmentVariable("SYSTEM_TO_EMAIL")
            ?? throw new InvalidOperationException(
                "SYSTEM_TO_EMAIL environment variable is not set."
            );
        string? sysEmailServer =
            Environment.GetEnvironmentVariable("SYSTEM_EMAIL_SERVER")
            ?? throw new InvalidOperationException(
                "SYSTEM_EMAIL_SERVER environment variable is not set."
            );
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("LuckyPennySoftware", LogEventLevel.Error)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", appName)
            .Enrich.WithProperty(
                "Environment",
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            )
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.File(
                new CompactJsonFormatter(),
                "Logs/log-.json",
                rollingInterval: RollingInterval.Day
            )
            .WriteTo.Email(
                from: sysFromEmail,
                to: sysToEmail,
                host: sysEmailServer,
                credentials: new NetworkCredential(
                    Environment.GetEnvironmentVariable("SYSTEM_EMAIL_USER"),
                    Environment.GetEnvironmentVariable("SYSTEM_EMAIL_PASSWORD")
                ),
                restrictedToMinimumLevel: LogEventLevel.Error
            )
            .CreateLogger();
        builder.Host.UseSerilog();
    }
}
