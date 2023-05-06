using Cronos;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SimpleCronWorkerService
{
    public abstract class CronWorkerService : BackgroundService
    {
        private PeriodicTimer? _periodicTimer;

        private readonly CronExpression _cronExpression;

        private readonly TimeZoneInfo _timeZone;

        public CronWorkerService(string cronExpression, TimeZoneInfo? timeZone = null)
        {
            _cronExpression = CronExpression.Parse(cronExpression);
            _timeZone = timeZone ?? TimeZoneInfo.Utc;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTimeOffset? nextOcurrence = _cronExpression.GetNextOccurrence(DateTimeOffset.UtcNow, _timeZone);

            if (nextOcurrence.HasValue)
            {
                var delay = nextOcurrence.Value - DateTimeOffset.UtcNow;
                _periodicTimer = new PeriodicTimer(delay);

                if (await _periodicTimer.WaitForNextTickAsync(stoppingToken))
                {
                    _periodicTimer.Dispose();
                    _periodicTimer = null;

                    await DoWork(stoppingToken);

                    await ExecuteAsync(stoppingToken);
                }
            }
        }

        protected abstract Task DoWork(CancellationToken stoppingToken);

    }
}
