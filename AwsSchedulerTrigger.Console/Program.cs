using AwsSchedulerTrigger.Console.SchedulerTrigger.Infrastructure.Extensions;
using AwsSchedulerTrigger.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Use our extension method for cleaner DI
        services.AddSchedulerClient(context.Configuration);
        services.AddTransient<App>();
    });

var host = builder.Build();

try
{
    await host.Services.GetRequiredService<App>().RunAsync(args);
    Environment.ExitCode = 0;
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.ToString()}");
    Environment.ExitCode = 1;
}