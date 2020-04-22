using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Models;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISubscriptionService
    {
        void Subscribe(string projectId, string subscriptionId, IMessageHandler messageHandler, ScheduleModel scheduleModel);
    }
}
