using SimpleCronWorkerService;

namespace WebApplicationApiSample.Services
{
    public class WorkerWithSemaphore : CronWorkerService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SemaphoreSlim _semaphoreSlim;

        public WorkerWithSemaphore(CronWorkerServiceSettings<WorkerWithSemaphore> cronWorkerServiceSettings, ILogger<Worker> logger)
            : base(cronWorkerServiceSettings.CronExpression, cronWorkerServiceSettings.TimeZone)
        {
            _logger = logger;
            _semaphoreSlim = new SemaphoreSlim(1);
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            if (!await _semaphoreSlim.WaitAsync(TimeSpan.FromMilliseconds(10)))
            {
                _logger.LogWarning("Previous instance has not yet finish, skipping");
                return;
            }

            var guid = Guid.NewGuid();
            _logger.LogInformation("Running WorkerWithSemaphore Task:{0}... at {1}", guid, DateTime.UtcNow);
            await Task.Delay(5000);
            _logger.LogInformation("Finished WorkerWithSemaphore Task:{0}... at {1}", guid, DateTime.UtcNow);
        }
    }
}
