namespace AwsSchedulerTrigger.Console.SchedulerTrigger.Core.Config
{
    public class SchedulerApiOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public Dictionary<string, SchedulerConfig> Schedulers { get; set; } = new();
    }
}
