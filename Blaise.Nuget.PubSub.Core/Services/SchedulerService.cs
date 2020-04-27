
using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Blaise.Nuget.PubSub.Core.Jobs;
using Quartz;
using Quartz.Impl;
using System;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SchedulerService : ISchedulerService
    {
        private ICronExpressionService _cronService;

        public SchedulerService(ICronExpressionService cronService)
        {
            _cronService = cronService;
        }

        public void Schedule(Action action, int intervalNumber, IntervalType intervalType)
        {

            var cronExpression = _cronService.GenerateCronExpression(intervalNumber, intervalType);

            var scheduler = GetScheduler();
            var job = CreateJob();
            var trigger = CreateTrigger(intervalNumber, intervalType);

            job.JobDataMap.Put("action", action);

            scheduler.ScheduleJob(job, trigger);
        }

        private IScheduler GetScheduler()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            return scheduler;
        }

        private IJobDetail CreateJob()
        {
            return JobBuilder.Create<SubscriptionJob>()
                           .WithIdentity("subscriptionJob", "subscriptionJobGroup")
                           .Build();
        }

        private ITrigger CreateTrigger(int intervalNumber, IntervalType intervalType)
        {
            return TriggerBuilder.Create()
                     .WithIdentity("subscriptionJobTrigger", "subscriptionJobTriggerGroup")
                     .StartNow()
                     .WithSimpleSchedule(x => x
                         .WithIntervalInSeconds(10)
                         .RepeatForever())
                     .Build();
        }
    }
}
