using Cronos;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCronWorkerService
{
    public abstract class CronWorkerService : BackgroundService
    {
        private PeriodicTimer? _timer;

        private readonly CronExpression _cronExpression;

        private readonly TimeZoneInfo _timeZone;

        public CronWorkerService(string cronExpression, TimeZoneInfo? timeZone = null)
        {
            _cronExpression = CronExpression.Parse(cronExpression);
            _timeZone = timeZone ?? TimeZoneInfo.Utc;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var next = _cronExpression.GetNextOccurrence(DateTimeOffset.Now, _timeZone);
                if (!next.HasValue)
                {
                    await ExecuteAsync(cancellationToken);
                    return;
                }

                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)
                {
                    await ExecuteAsync(cancellationToken);
                    return;
                }

                _timer = new PeriodicTimer(delay);
                if (await _timer.WaitForNextTickAsync())
                {
                    _timer.Dispose();
                    _timer = null;

                    var doWorktask = DoWork(cancellationToken);
                    var executeTask = ExecuteAsync(cancellationToken);

                    await doWorktask;
                    await executeTask;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("CronWorkerService was cancelled");
            }
        }

        protected abstract Task DoWork(CancellationToken cancellationToken);

    }
}
