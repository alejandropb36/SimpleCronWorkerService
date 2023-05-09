using System;

namespace SimpleCronWorkerService
{
    public interface ICronWorkerServiceSettings
    {
        public string CronExpression { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
        public bool CronExpressionIncludeSeconds { get; set; }
    }

    public class CronWorkerServiceSettings<T> : ICronWorkerServiceSettings where T : CronWorkerService
    {
        public string CronExpression { get; set; } = @"* * * * *";
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
        public bool CronExpressionIncludeSeconds { get; set; } = false;
    }
}
