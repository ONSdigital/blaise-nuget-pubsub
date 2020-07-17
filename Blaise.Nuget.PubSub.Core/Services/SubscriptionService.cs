using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using System.Linq;
using Blaise.Nuget.PubSub.Core.Models;
using Google.Protobuf.WellKnownTypes;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IDeadLetterService _deadLetterService;

        private SubscriberServiceApiClient _subscriberServiceClient;

        public SubscriptionService(IDeadLetterService deadLetterService)
        {
            _deadLetterService = deadLetterService;
        }

        public Subscription CreateSubscription(string projectId, string topicId, string subscriptionId,
            SubscriptionSettingsModel settingsModel)
        {
            var client = GetSubscriberClient();

            if (SubscriptionExists(projectId, subscriptionId))
            {
                return GetSubscription(projectId, subscriptionId);
            }

            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            var topicName = new TopicName(projectId, topicId);
            var subscription = client.CreateSubscription(subscriptionName, topicName, null, settingsModel.AckTimeoutInSeconds);

            if (settingsModel.RetrySettings != null)
            {
                AddRetrySettingsToSubscription(subscription, projectId, topicId, settingsModel);
            }

            return subscription;
        }

        public void DeleteSubscription(string projectId, string subscriptionId)
        {
            var client = GetSubscriberClient();

            if (!SubscriptionExists(projectId, subscriptionId))
            {
                return;
            }

            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            client.DeleteSubscription(subscriptionName);
        }

        public Subscription GetSubscription(string projectId, string subscriptionId)
        {
            var client = GetSubscriberClient();

            return client.GetSubscription(new SubscriptionName(projectId, subscriptionId));
        }

        public bool SubscriptionExists(string projectId, string subscriptionId)
        {
            var client = GetSubscriberClient();

            var projectName = new ProjectName(projectId);
            var subscriptions = client.ListSubscriptions(projectName);

            return subscriptions.Any(s => s.SubscriptionName.SubscriptionId == subscriptionId);
        }

        private SubscriberServiceApiClient GetSubscriberClient()
        {
            return _subscriberServiceClient ?? (_subscriberServiceClient = SubscriberServiceApiClient.Create());
        }

        private void AddRetrySettingsToSubscription(Subscription subscription, string projectId, string topicId,
            SubscriptionSettingsModel settingsModel)
        {
            subscription.RetryPolicy = new RetryPolicy
            {
                MinimumBackoff = new Duration { Seconds = settingsModel.RetrySettings.MinimumBackOffInSeconds },
                MaximumBackoff = new Duration { Seconds = settingsModel.RetrySettings.MaximumBackOffInSeconds }
            };

            subscription.DeadLetterPolicy = _deadLetterService.CreateDeadLetterPolicy(projectId, topicId,
                settingsModel.RetrySettings.MaximumDeliveryAttempts);
        }
    }
}


