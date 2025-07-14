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
        private Timer _timer;

        private readonly CronExpression _cronExpression;

        private readonly TimeZoneInfo _timeZone;

        private const int DelayMaxValueMilliseconds = (int.MaxValue - (60 * 1000));

        public CronWorkerService(ICronWorkerServiceSettings settings)
        {
            _cronExpression = settings.CronExpressionIncludeSeconds
                ? CronExpression.Parse(settings.CronExpression, CronFormat.IncludeSeconds)
                : CronExpression.Parse(settings.CronExpression);

            _timeZone = settings.TimeZone ?? TimeZoneInfo.Utc;

            _timer = new Timer
            {
                AutoReset = false
            };
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await RestartTimer(cancellationToken);

            _timer.Elapsed += async (sender, args) =>
            {
                _timer.Stop();

                if (!cancellationToken.IsCancellationRequested)
                {
                    var doWorkTask = DoWork(cancellationToken);

                    var restartTimerTask = RestartTimer(cancellationToken);

                    await doWorkTask;
                    await restartTimerTask;
                }
            };

            await Task.CompletedTask;
        }

        private async Task RestartTimer(CancellationToken cancellationToken)
        {
            var next = _cronExpression.GetNextOccurrence(DateTimeOffset.Now, _timeZone);
            if (!next.HasValue)
            {
                return;
            }

            var delay = GetDelay(next.Value);

            // If it is a value greater than the MaxValue of Int,
            // we have to wait for this difference to decrease.
            // I wait for the same amount as MaxValue minus one minute and
            // then validate again if it could be a valid value for the Timer.
            while (delay.TotalMilliseconds > int.MaxValue)
            {
                await Task.Delay(DelayMaxValueMilliseconds, cancellationToken);
                delay = GetDelay(next.Value);
            }

            if (delay.TotalMilliseconds <= 0)
            {
                await RestartTimer(cancellationToken);
                return;
            }

            _timer.Interval = delay.TotalMilliseconds;
            _timer.Start();
        }

        private TimeSpan GetDelay(DateTimeOffset nextValue) => nextValue - DateTimeOffset.Now;

        protected abstract Task DoWork(CancellationToken cancellationToken);

    }
}
