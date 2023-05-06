using SimpleCronWorkerService;
using WorkerServiceSample;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddCronWorkerService<Worker>(options =>
        {
            // Run every minute
            options.CronExpression = "* * * * *";
            options.TimeZone = TimeZoneInfo.Local;
        });
    })
    .Build();

await host.RunAsync();
