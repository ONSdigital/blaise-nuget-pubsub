using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.PubSub.V1;
using Nito.AsyncEx.Synchronous;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SubscriberService : ISubscriberService
    {
        private SubscriberClient _subscriberClient;

        public void StartConsuming(string projectId, string subscriptionId, IMessageHandler messageHandler)
        {
            var createSubscriptionTask = StartConsumingAsync(projectId, subscriptionId, messageHandler);
            createSubscriptionTask.WaitAndUnwrapException();
        }

        public void StopConsuming()
        {
            var cancelSubscriptionTask = StopConsumingAsync();
            cancelSubscriptionTask.WaitAndUnwrapException();
        }

        private async Task StartConsumingAsync(string projectId, string subscriptionId, IMessageHandler messageHandler)
        {
            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            _subscriberClient = await SubscriberClient.CreateAsync(subscriptionName);

            await _subscriberClient.StartAsync((msg, cancellationToken) =>
            {               
                var message = msg.Data.ToStringUtf8();

                if (messageHandler.HandleMessage(message))
                {
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }

                return Task.FromResult(SubscriberClient.Reply.Nack);
            });
        }

        private async Task StopConsumingAsync()
        {
            if(_subscriberClient == null)
            {
                throw new InvalidOperationException("No subscriptons have been setup");
            }

            await _subscriberClient.StopAsync(CancellationToken.None);
        }
    }
}


