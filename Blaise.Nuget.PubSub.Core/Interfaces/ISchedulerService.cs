using Blaise.Nuget.PubSub.Contracts.Enums;
using System;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISchedulerService
    {
        void Schedule(Action action, int intervalNumber, IntervalType intervalType);
    }
}
