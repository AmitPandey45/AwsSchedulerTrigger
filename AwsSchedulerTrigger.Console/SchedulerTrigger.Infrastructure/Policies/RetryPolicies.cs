namespace AwsSchedulerTrigger.Console.SchedulerTrigger.Infrastructure.Policies
{
    using Polly;
    using Polly.Extensions.Http;
    using System;

    public static class RetryPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var baseDelayMs = 5000; // 5 seconds base
            var jitterer = new Random();

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                retryCount: 3,
                retryAttempt =>
                {
                    // Exponential backoff: 5s, 10s, 20s + jitter up to 5s
                    var exponentialDelayMs = baseDelayMs * Math.Pow(2, retryAttempt - 1);
                    var jitterMs = jitterer.NextDouble() * baseDelayMs;

                    return TimeSpan.FromMilliseconds(exponentialDelayMs + jitterMs);
                },
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine(
                        $"Retry {retryAttempt} after {timespan.TotalSeconds:F2}s due to " +
                        $"{outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                });
        }
    }
}
