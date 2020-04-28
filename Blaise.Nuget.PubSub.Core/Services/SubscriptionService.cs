using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Api.Gax.Grpc;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using Nito.AsyncEx.Synchronous;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private SubscriberClient _subscriberClient;
        private SubscriberServiceApiClient _subscriberServiceClient;

        public SubscriptionService()
        {
            _subscriberServiceClient = SubscriberServiceApiClient.Create();
        }

        public void StartConsuming(string projectId, string subscriptionId, IMessageHandler messageHandler)
        {
            var createSubscriptionTask = CreateSubscriptionAsync(projectId, subscriptionId, messageHandler);
            createSubscriptionTask.WaitAndUnwrapException();
        }

        public void StopConsuming()
        {
            var cancelSubscriptionTask = CancelSubscriptionAsync();
            cancelSubscriptionTask.WaitAndUnwrapException();
        }

        public void CreateSubscription(string projectId, string topicId, string subscriptionId, int ackDeadlineInSeconds = 600)
        {
            if (SubscriptionExists(projectId, subscriptionId))
            {
                return;
            }

            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            var topicName = new TopicName(projectId, topicId);
            _subscriberServiceClient.CreateSubscription(subscriptionName, topicName, pushConfig: null, ackDeadlineSeconds: ackDeadlineInSeconds);
        }

        public void DeleteSubscription(string projectId, string subscriptionId)
        {
            if (!SubscriptionExists(projectId, subscriptionId))
            {
                return;
            }

            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            _subscriberServiceClient.DeleteSubscription(subscriptionName);
        }

        public bool SubscriptionExists(string projectId, string subscriptionId)
        {
            var projectName = new ProjectName(projectId);
            var subscriptions = _subscriberServiceClient.ListSubscriptions(projectName);

            return subscriptions.Any(s => s.SubscriptionName.SubscriptionId == subscriptionId);
        }

        private async Task CreateSubscriptionAsync(string projectId, string subscriptionId, IMessageHandler messageHandler)
        {
            var subscriptionName = new SubscriptionName(projectId, subscriptionId);

            _subscriberClient = await SubscriberClient.CreateAsync(subscriptionName);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _subscriberClient.StartAsync((msg, cancellationToken) =>
            {
                var message = msg.Data.ToStringUtf8();

                if (messageHandler.HandleMessage(message))
                {
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }
                return Task.FromResult(SubscriberClient.Reply.Nack);               
            }).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task CancelSubscriptionAsync()
        {
            if(_subscriberClient == null)
            {
                throw new InvalidOperationException("No subscriptons have been setup");
            }

            await _subscriberClient.StopAsync(CancellationToken.None);
        }
    }
}


