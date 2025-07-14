using SimpleCronWorkerService;

namespace SimpleCronWorkerServiceTests
{
    public class TestCronWorkerService : CronWorkerService
    {
        public bool WorkExecuted { get; private set; } = false;

        public TestCronWorkerService(ICronWorkerServiceSettings settings) : base(settings) { }

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            WorkExecuted = true;
            return Task.CompletedTask;
        }
    }

    public class TestCronWorkerServiceSettings : ICronWorkerServiceSettings
    {
        public string CronExpression { get; set; } = "* * * * * *";
        public bool CronExpressionIncludeSeconds { get; set; } = true;
        public TimeZoneInfo? TimeZone { get; set; } = TimeZoneInfo.Utc;
    }

    public class CronWorkerServiceTests
    {
        [Fact]
        public async Task CronWorkerService_ShouldExecute_DoWork()
        {
            // Arrange
            var settings = new TestCronWorkerServiceSettings
            {
                CronExpression = "* * * * * *",
                CronExpressionIncludeSeconds = true
            };

            var service = new TestCronWorkerService(settings);

            // Act
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(2));
            await service.StartAsync(cts.Token);

            await Task.Delay(2500);

            // Assert
            Assert.True(service.WorkExecuted);
        }
    }
}