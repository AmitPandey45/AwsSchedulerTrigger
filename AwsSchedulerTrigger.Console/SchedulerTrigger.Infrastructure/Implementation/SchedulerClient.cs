namespace AwsSchedulerTrigger.Console.SchedulerTrigger.Infrastructure.Implementation
{
    using AwsSchedulerTrigger.Console.SchedulerTrigger.Core.Config;
    using AwsSchedulerTrigger.Console.SchedulerTrigger.Core.Interface;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class SchedulerClient : ISchedulerClient
    {
        private readonly HttpClient _httpClient;
        private readonly SchedulerApiOptions _options;
        private readonly ILogger<SchedulerClient> _logger;

        public SchedulerClient(
            HttpClient httpClient,
            IOptions<SchedulerApiOptions> options,
            ILogger<SchedulerClient> logger)
        {
            this._httpClient = httpClient;
            this._options = options.Value;
            this._logger = logger;
        }

        public async Task<string> TriggerAsync(string schedulerName, string apiKey)
        {
            this._logger.LogInformation($"Calling scheduler: {schedulerName}");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("API key is required");
            }

            if (!this._options.Schedulers.TryGetValue(schedulerName, out var scheduler))
            {
                throw new ArgumentException($"Scheduler '{schedulerName}' not found.");
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, scheduler.Endpoint);
            request.Headers.Add("x-api-key", apiKey);
            var response = await this._httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            //// response.EnsureSuccessStatusCode();

            // Log full response first
            this._logger.LogInformation($"Response Status: {(int)response.StatusCode} ({response.StatusCode})");
            this._logger.LogInformation($"Response Body: {responseBody}");

            // Now log based on status code
            if (response.IsSuccessStatusCode)
            {
                this._logger.LogInformation("Request succeeded.");
            }
            else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                this._logger.LogWarning(
                    "HTTP client error. StatusCode: {StatusCode} ({StatusName})",
                    (int)response.StatusCode,
                    response.StatusCode);
            }
            else if ((int)response.StatusCode >= 500)
            {
                this._logger.LogCritical(
                    "HTTP server error. StatusCode: {StatusCode} ({StatusName})",
                    (int)response.StatusCode,
                    response.StatusCode);
            }

            return responseBody;
        }
    }
}
