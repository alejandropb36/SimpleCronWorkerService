using SimpleCronWorkerService;

namespace WebApplicationApiSample.Services
{
    public class Worker2 : CronWorkerService
    {
        private readonly ILogger<Worker2> _logger;

        public Worker2(CronWorkerServiceSettings<Worker2> cronWorkerServiceSettings, ILogger<Worker2> logger)
            : base(cronWorkerServiceSettings.CronExpression, cronWorkerServiceSettings.TimeZone)
        {
            _logger = logger;
        }

        protected override Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Running Worker2... at {0}", DateTime.UtcNow);


            return Task.CompletedTask;
        }
    }
}
