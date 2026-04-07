using AwsSchedulerTrigger.Console.SchedulerTrigger.Infrastructure.Extensions;
using AwsSchedulerTrigger.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MsLogLevel = Microsoft.Extensions.Logging.LogLevel;
using NLog;
using NLog.Extensions.Hosting;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{
    var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(MsLogLevel.Trace);
    })
    .UseNLog()
    .ConfigureServices((context, services) =>
    {
        // Use our extension method for cleaner DI
        services.AddSchedulerClient(context.Configuration);
        services.AddTransient<App>();
    });

    var host = builder.Build();

    await host.Services.GetRequiredService<App>().RunAsync(args);
    Environment.ExitCode = 0;
}
catch (Exception ex)
{
    logger.Error(ex, "Application failed");
    Environment.ExitCode = 1;
}
finally
{
    LogManager.Shutdown();
}