using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using FIXME;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateLogger();

IHost app = Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        // Clear providers and let Serilog dictate log levels and filters
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Trace);
    })
    .UseSerilog(Log.Logger)
    .ConfigureServices(services =>
    {
        services.AddOpenTelemetry(typeof(Program).Assembly.GetName());
    })
    .Build();

Log.Logger.Information("Starting application");

try
{
    app.Run();
}
catch (Exception e)
{
    Log.Logger.Fatal(e, "Application exited due to an unhandled exception.");
    return -1;
}
finally
{
    Log.Logger.Information("Application shutdown");
    Log.CloseAndFlush();
}

return 0;
