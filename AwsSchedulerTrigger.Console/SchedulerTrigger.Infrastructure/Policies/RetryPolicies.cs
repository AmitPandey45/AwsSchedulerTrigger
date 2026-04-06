namespace AwsSchedulerTrigger.Console.SchedulerTrigger.Infrastructure.Policies
{
    using Polly;
    using Polly.Extensions.Http;
    using System;

    public static class RetryPolicies
    {
        private const int MaxDelayMs = 180_000;
        private const int BaseDelayMs = 5000;
        private static readonly Random _rng = new Random();

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                retryCount: 3,
                retryAttempt =>
                {
                    return TimeSpan.FromMilliseconds(CalculateDelay(attempt: retryAttempt));
                },
                onRetry: async (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine(
                        $"[Async Retry] Attempt {retryAttempt} waiting {timespan.TotalSeconds:F2}s due to " +
                        $"{outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                    await Task.CompletedTask;
                });
        }

        public static int CalculateDelay(int attempt, int baseDelayMs = 5000)
        {
            // Exponential backoff: delay * (2^(attempt-1))
            double expoBackoff = Math.Pow(2, attempt - 1) * baseDelayMs;

            int delay = (int)Math.Min(expoBackoff, MaxDelayMs);

            // jitter: +/- up to 10% of delay to avoid thundering herd
            int jitterRange = (int)(delay * 0.1);
            int jitterOffset = _rng.Next(-jitterRange, jitterRange + 1);

            int finalDelay = delay + jitterOffset;

            // Avoid negative delays
            return Math.Max(0, finalDelay);
        }
    }
}
