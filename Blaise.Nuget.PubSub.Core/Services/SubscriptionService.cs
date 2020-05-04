using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ITopicService _topicService;

        private readonly SubscriberServiceApiClient _subscriberServiceClient;

        public SubscriptionService(ITopicService topicService)
        {
            _topicService = topicService;
            _subscriberServiceClient = SubscriberServiceApiClient.Create();
        }

        public Subscription CreateSubscription(string projectId, string topicId, string subscriptionId, int messageTimeoutInSeconds)
        {
            if(messageTimeoutInSeconds < 10 || messageTimeoutInSeconds > 600)
            {
                throw new ArgumentOutOfRangeException("The deadline for acking messages must be between '1' and '600'");
            }

            if (SubscriptionExists(projectId, subscriptionId))
            {
                return GetSubscription(projectId, subscriptionId);
            }

            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            var topicName = new TopicName(projectId, topicId);
            return _subscriberServiceClient.CreateSubscription(subscriptionName, topicName, pushConfig: null, ackDeadlineSeconds: messageTimeoutInSeconds);
        }

        public Subscription CreateSubscription(string projectId, string topicId, string subscriptionId, int messageTimeoutInSeconds, int maxDeliveryAttempts)
        {
            if (maxDeliveryAttempts < 5|| maxDeliveryAttempts > 100)
            {
                throw new ArgumentOutOfRangeException("The maximum number of retries for processing messages must be between '5' and '100'");
            }

            var subscription = CreateSubscription(projectId, topicId, subscriptionId, messageTimeoutInSeconds);
            var deadletterTopicId = $"{topicId}-deadletter";
            var topic = _topicService.CreateTopic(projectId, deadletterTopicId);

            return UpdateSubscriptionWithDeadLetterPolicy(subscription, topic, maxDeliveryAttempts);
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

        public Subscription GetSubscription(string projectId, string subscriptionId)
        {
            return _subscriberServiceClient.GetSubscription(new SubscriptionName(projectId, subscriptionId));
        }

        public bool SubscriptionExists(string projectId, string subscriptionId)
        {
            var projectName = new ProjectName(projectId);
            var subscriptions = _subscriberServiceClient.ListSubscriptions(projectName);

            return subscriptions.Any(s => s.SubscriptionName.SubscriptionId == subscriptionId);
        }

        private Subscription UpdateSubscriptionWithDeadLetterPolicy(Subscription subscription, Topic deadLetterTopic, int maxDeliveryAttempts)
        {
            subscription.DeadLetterPolicy = new DeadLetterPolicy
            {
                DeadLetterTopic = deadLetterTopic.Name,
                MaxDeliveryAttempts = maxDeliveryAttempts
            };

            return _subscriberServiceClient.UpdateSubscription(
                new UpdateSubscriptionRequest { 
                    Subscription = subscription, 
                    UpdateMask =  FieldMask.FromFieldNumbers<Subscription>(new List<int> { 13 }) 
                });
        }
    }
}


