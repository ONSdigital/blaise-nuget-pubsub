using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using System.Linq;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriberServiceApiClient _subscriberServiceClient;

        public SubscriptionService()
        {
            _subscriberServiceClient = SubscriberServiceApiClient.Create();
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
    }
}


