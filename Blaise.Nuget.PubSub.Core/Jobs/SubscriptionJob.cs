using Quartz;
using System;

namespace Blaise.Nuget.PubSub.Core.Jobs
{
    public class SubscriptionJob : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            var action = (Action)dataMap["action"];
            action();
        }
    }
}
