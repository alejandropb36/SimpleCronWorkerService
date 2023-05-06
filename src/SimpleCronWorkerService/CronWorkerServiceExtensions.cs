using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleCronWorkerService
{
    public static class CronWorkerServiceExtensions
    {
        public static IServiceCollection AddCronWorkerService<T>(this IServiceCollection services, Action<CronWorkerServiceSettings<T>> options)
        where T : CronWorkerService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var config = new CronWorkerServiceSettings<T>();
            options.Invoke(config);

            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(CronWorkerServiceSettings<T>.CronExpression));
            }

            services.AddSingleton<CronWorkerServiceSettings<T>>(config);
            services.AddHostedService<T>();

            return services;
        }
    }
}
