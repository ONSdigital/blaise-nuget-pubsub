using Blaise.Nuget.PubSub.Contracts.Interfaces;
using System;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISubscriptionService
    {
        void StartConsuming(string projectId, string subscriptionId, IMessageHandler messageHandler);

        void StopConsuming();
    }
}
