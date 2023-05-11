# SimpleCronWorkerService

[![NuGet](https://img.shields.io/nuget/v/SimpleCronWorkerService.svg)](https://www.nuget.org/packages/SimpleCronWorkerService) [![Release - Build, pack and publish NuGet](https://github.com/alejandropb36/SimpleCronWorkerService/actions/workflows/release-nuget.yml/badge.svg)](https://github.com/alejandropb36/SimpleCronWorkerService/actions/workflows/release-nuget.yml) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://raw.githubusercontent.com/alejandropb36/SimpleCronWorkerService/main/LICENSE)

A simple and easy way to implement worker services scheduled by a CRON expression

## Getting started

### Package Install

Installing is as easy as: `dotnet add package SimpleCronWorkerService` or `Install-Package SimpleCronWorkerService` depending on your setup.

### Create your CronWorkerService

1. Create your Worker class which must inherit from the abstract class `CronWorkerService` imported from `SimpleCronWorkerService`
    ```csharp
    using SimpleCronWorkerService;

    namespace WorkerServiceSample
    {
        public class Worker : CronWorkerService
        {
        }
    }
    ```

2. In the constructor, as the first parameter, you will receive a `CronWorkerServiceSettings<>` object as the type of your worker class. These settings must be passed to the base constructor `: base(cronWorkerServiceSettings)`.
    ```csharp
    using SimpleCronWorkerService;

    namespace WorkerServiceSample
    {
        public class Worker : CronWorkerService
        {
            private readonly ILogger<Worker> _logger;

            public Worker(CronWorkerServiceSettings<Worker> cronWorkerServiceSettings,ILogger<Worker> logger)
                :base(cronWorkerServiceSettings)
            {
                _logger = logger;
            }
        }
    }
    ```

3. You have to override the method `protected override Task DoWork(CancellationToken stoppingToken)` with the logic that you want your Worker to execute.
    ```csharp
    using SimpleCronWorkerService;

    namespace WorkerServiceSample
    {
        public class Worker : CronWorkerService
        {
            private readonly ILogger<Worker> _logger;

            public Worker(CronWorkerServiceSettings<Worker> cronWorkerServiceSettings,ILogger<Worker> logger)
                :base(cronWorkerServiceSettings)
            {
                _logger = logger;
            }

            protected override Task DoWork(CancellationToken cancellationToken)
            {
                // ... Your logic
            }
        }
    }
    ```

Example
```csharp
using SimpleCronWorkerService;

namespace WorkerServiceSample
{
    public class Worker : CronWorkerService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(CronWorkerServiceSettings<Worker> cronWorkerServiceSettings,ILogger<Worker> logger)
            :base(cronWorkerServiceSettings)
        {
            _logger = logger;
        }

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running... at {0}", DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}
```

### Register your CronWorkerService

In your service container, you can add your worker `using SimpleCronWorkerService` with the method `Services.AddCronWorkerService<>`

The type `<>` should be your `Worker` class.

```csharp
using SimpleCronWorkerService;
 ...
// Add services to the container.
builder.Services.AddCronWorkerService<Worker>(options =>
{
    // Run every minute
    options.CronExpression = @"* * * * *";
    options.TimeZone = TimeZoneInfo.Local;
});
```

Inside this method, the options are passed, these options are of type `CronWorkerServiceSettings<T>`

```csharp
public class CronWorkerServiceSettings<T> : ICronWorkerServiceSettings where T : CronWorkerService
{
    public string CronExpression { get; set; } = @"* * * * *";
    public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
    public bool CronExpressionIncludeSeconds { get; set; } = false;
}
```

The `CronExpression` is a string and we are using the Cronos library. For more information about this syntax, please visit the [Cronos documentation](https://github.com/HangfireIO/Cronos#cron-format). By default, it is the expression for every minute (`"* * * * *"`).

The `TimeZone` sets the time zone in which you want your worker to run. By default, it is `UTC`.

The `CronExpressionIncludeSeconds` is a boolean used if you are going to use the seconds part of the expression ([Cronos documentation](https://github.com/HangfireIO/Cronos#cron-format)). By default, it is `false`.

## Samples
- [WebApplicationApiSample](https://github.com/alejandropb36/SimpleCronWorkerService/tree/main/samples/WebApplicationApiSample)
- [WorkerServiceSample](https://github.com/alejandropb36/SimpleCronWorkerService/tree/main/samples/WorkerServiceSample)
- [WorkerWithSemaphore](https://github.com/alejandropb36/SimpleCronWorkerService/blob/main/samples/WebApplicationApiSample/Services/WorkerWithSemaphore.cs)

## Contributing

Please fork this repo then create a PR from the fork into the original one. 