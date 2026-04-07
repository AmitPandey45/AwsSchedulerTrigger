namespace AwsSchedulerTrigger.ConsoleApp
{
    using AwsSchedulerTrigger.Console.SchedulerTrigger.Core.Interface;
    using Microsoft.Extensions.Logging;
    using System;

    public class App
    {
        private readonly ISchedulerClient _client;
        private readonly ILogger<App> _logger;

        public App(ISchedulerClient client, ILogger<App> logger)
        {
            this._client = client;
            this._logger = logger;
        }

        public async Task RunAsync(string[] args)
        {
            this._logger.LogInformation("App started");
            if (args.Length < 2)
            {
                this._logger.LogInformation("Usage: <exe> <SchedulerName> <ApiKey>");
                throw new ArgumentException("Invalid arguments");
            }

            var schedulerName = args[0];
            var apiKey = args[1];

            this._logger.LogInformation($"Triggering scheduler: {schedulerName}");

            var result = await this._client.TriggerAsync(schedulerName, apiKey);

            this._logger.LogInformation($"Response: {result}");
            this._logger.LogInformation("App finished");
        }
    }
}
