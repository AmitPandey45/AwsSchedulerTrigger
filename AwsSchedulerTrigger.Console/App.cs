namespace AwsSchedulerTrigger.ConsoleApp
{
    using AwsSchedulerTrigger.Console.SchedulerTrigger.Core.Interface;
    using System;

    public class App
    {
        private readonly ISchedulerClient _client;

        public App(ISchedulerClient client)
        {
            this._client = client;
        }

        public async Task RunAsync(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: <exe> <SchedulerName> <ApiKey>");
                throw new ArgumentException("Invalid arguments");
            }

            var schedulerName = args[0];
            var apiKey = args[1];

            Console.WriteLine($"Triggering scheduler: {schedulerName}");

            var result = await this._client.TriggerAsync(schedulerName, apiKey);

            Console.WriteLine($"Response: {result}");
        }
    }
}
