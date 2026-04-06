namespace AwsSchedulerTrigger.Console.SchedulerTrigger.Core.Interface
{
    public interface ISchedulerClient
    {
        Task<string> TriggerAsync(string schedulerName, string apiKey);
    }
}
