using SimpleCronWorkerService;

namespace WebApplicationApiSample.Services
{
    public class Worker : CronWorkerService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(CronWorkerServiceSettings<Worker> cronWorkerServiceSettings, ILogger<Worker> logger)
            : base(cronWorkerServiceSettings.CronExpression, cronWorkerServiceSettings.TimeZone)
        {
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            var guid = Guid.NewGuid();
            _logger.LogInformation("Running Worker1 Task:{0}... at {1}", guid, DateTime.UtcNow);
            await Task.Delay(5000);
            _logger.LogWarning("Finished Worker1 Task:{0}... at {1}", guid, DateTime.UtcNow);
        }
    }
}
