using SimpleCronWorkerService;

namespace WebApplicationApiSample.Services
{
    public class Worker : CronWorkerService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(CronWorkerServiceSettings<Worker> cronWorkerServiceSettings, ILogger<Worker> logger)
            : base(cronWorkerServiceSettings)
        {
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            var guid = Guid.NewGuid();
            _logger.LogInformation("Running Worker1 at {date} - Task:{guid}", DateTime.UtcNow, guid);
            await Task.Delay(5000);
            _logger.LogTrace("Finished Worker1 at {date} - Task:{guid}", DateTime.UtcNow, guid);
        }
    }
}
