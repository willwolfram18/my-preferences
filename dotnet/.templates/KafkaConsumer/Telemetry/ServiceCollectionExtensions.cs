using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace FIXME;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, AssemblyName startup,
        IConfiguration? configuration = default,
        Action<TracerProviderBuilder>? tracerConfigure = default)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        var activitySource = new ActivitySource(
            startup.Name!.ToLower(),
            (startup.Version ?? new Version(0, 1, 0)).ToString());

        services.AddSingleton(activitySource)
            .AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder.AddSource(activitySource.Name);

                if (configuration is not null)
                {
                    switch (configuration["OpenTelemetry:Exporter"]?.ToLower())
                    {
                        case "console":
                            builder.AddConsoleExporter();
                            break;
                    }
                }

                tracerConfigure?.Invoke(builder);
            });

        return services;
    }
}
