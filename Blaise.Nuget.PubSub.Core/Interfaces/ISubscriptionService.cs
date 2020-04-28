using Blaise.Nuget.PubSub.Contracts.Interfaces;
using System;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISubscriptionService
    {
        void CreateSubscription(string projectId, string topicId, string subscriptionId, int ackDeadlineInSeconds = 600);

        void StartConsuming(string projectId, string subscriptionId, IMessageHandler messageHandler);

        void StopConsuming();
    }
}
