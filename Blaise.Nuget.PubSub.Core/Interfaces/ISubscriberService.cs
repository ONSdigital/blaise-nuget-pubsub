using Blaise.Nuget.PubSub.Contracts.Interfaces;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISubscriberService
    {
        void StartConsuming(string projectId, string subscriptionId, IMessageHandler messageHandler, bool throttle = false);

        void StartConsuming(string projectId, string subscriptionId, IMessageTriggerHandler messageHandler);

        void StopConsuming();
    }
}
