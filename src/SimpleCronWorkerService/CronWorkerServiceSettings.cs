using System;

namespace SimpleCronWorkerService
{
    public class CronWorkerServiceSettings<T>
    {
        public string CronExpression { get; set; } = string.Empty;
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
    }
}
