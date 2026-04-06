namespace AwsSchedulerTrigger.Console.SchedulerTrigger.Infrastructure.Extensions
{
    using AwsSchedulerTrigger.Console.SchedulerTrigger.Core.Config;
    using AwsSchedulerTrigger.Console.SchedulerTrigger.Core.Interface;
    using AwsSchedulerTrigger.Console.SchedulerTrigger.Infrastructure.Implementation;
    using AwsSchedulerTrigger.Console.SchedulerTrigger.Infrastructure.Policies;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSchedulerClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SchedulerApiOptions>(configuration.GetSection("SchedulerApi"));

            services.AddHttpClient<ISchedulerClient, SchedulerClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<SchedulerApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(RetryPolicies.GetRetryPolicy());

            return services;
        }
    }
}
