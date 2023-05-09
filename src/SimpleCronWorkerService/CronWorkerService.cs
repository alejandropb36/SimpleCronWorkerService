using Cronos;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace SimpleCronWorkerService
{
    public abstract class CronWorkerService : BackgroundService
    {
        private Timer? _timer;

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
                    return;
                }

                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)
                {
                    await ExecuteAsync(cancellationToken);
                    return;
                }

                _timer = new Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        var doWorkTask = DoWork(cancellationToken);
                    var executeTask = ExecuteAsync(cancellationToken);

                        await doWorkTask;
                    await executeTask;
                }
                };
                _timer.Start();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("CronWorkerService was cancelled");
            }
        }

        protected abstract Task DoWork(CancellationToken cancellationToken);

    }
}
