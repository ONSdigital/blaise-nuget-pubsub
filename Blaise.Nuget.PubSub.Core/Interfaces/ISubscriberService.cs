using Blaise.Nuget.PubSub.Contracts.Interfaces;
using System;
using System.Threading.Tasks;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISubscriberService
    {
        void StartConsuming(string projectId, string subscriptionId, IMessageHandler messageHandler, int stopConsumingAfterSeconds = 0);

        Task StartConsumingAsync(string projectId, string subscriptionId, IMessageHandler messageHandler, int stopConsumingAfterSeconds = 0);

        void StopConsuming();

        Task StopConsumingAsync();
    }
}
