using Blaise.Nuget.PubSub.Contracts.Interfaces;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISubscriptionService
    {
        void Consume(string projectId, string subscriptionId, IMessageHandler messageHandler, int numberOfMessages);
    }
}
