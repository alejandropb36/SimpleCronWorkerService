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

        private const int DelayMaxValueMilliseconds = (int.MaxValue - (10 * 1000));

        public CronWorkerService(ICronWorkerServiceSettings settings)
        {
            _cronExpression = settings.CronExpressionIncludeSeconds
                ? CronExpression.Parse(settings.CronExpression, CronFormat.IncludeSeconds)
                : CronExpression.Parse(settings.CronExpression);

            _timeZone = settings.TimeZone ?? TimeZoneInfo.Utc;
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

                if (delay.TotalMilliseconds > int.MaxValue)
                {
                    await Task.Delay(DelayMaxValueMilliseconds, cancellationToken);
                    delay = next.Value - DateTimeOffset.Now;
                }

                if (delay.TotalMilliseconds <= 0 )
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

            await Task.CompletedTask;
        }

        protected abstract Task DoWork(CancellationToken cancellationToken);

    }
}
