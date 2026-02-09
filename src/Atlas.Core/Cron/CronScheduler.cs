using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;

namespace Atlas.Core.Cron;

public class CronScheduler
{
    private readonly IScheduler _scheduler;
    private readonly ILogger<CronScheduler> _logger;

    private CronScheduler(IScheduler scheduler, ILogger<CronScheduler> logger)
    {
        _scheduler = scheduler;
        _logger = logger;
    }

    public static async Task<CronScheduler> CreateAsync(ILogger<CronScheduler> logger)
    {
        var factory = new StdSchedulerFactory();
        var scheduler = await factory.GetScheduler();
        return new CronScheduler(scheduler, logger);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _scheduler.Start(cancellationToken);
        _logger.LogInformation("Cron scheduler started");
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await _scheduler.Shutdown(cancellationToken);
        _logger.LogInformation("Cron scheduler stopped");
    }

    public async Task ScheduleJobAsync<T>(string cronExpression, string jobName, string groupName = "default") where T : IJob
    {
        var job = JobBuilder.Create<T>()
            .WithIdentity(jobName, groupName)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{jobName}-trigger", groupName)
            .WithCronSchedule(cronExpression)
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
        _logger.LogInformation("Scheduled job {JobName} with cron expression {CronExpression}", jobName, cronExpression);
    }
}
