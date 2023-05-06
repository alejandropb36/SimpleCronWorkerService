using SimpleCronWorkerService;

namespace WorkerServiceSample
{
    public class Worker : CronWorkerService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(CronWorkerServiceSettings<Worker> cronWorkerServiceSettings,ILogger<Worker> logger)
            :base(cronWorkerServiceSettings.CronExpression, cronWorkerServiceSettings.TimeZone)
        {
            _logger = logger;
        }

        protected override Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Running... at {0}", DateTime.UtcNow);
            Thread.Sleep(60000);
            _logger.LogInformation("End... at {0}", DateTime.UtcNow);


            return Task.CompletedTask;
        }
    }
}