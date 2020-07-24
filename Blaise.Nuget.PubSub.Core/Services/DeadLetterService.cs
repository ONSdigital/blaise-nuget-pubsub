using System;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class DeadLetterService : IDeadLetterService
    {
        private readonly ITopicService _topicService;
        private readonly ISubscriptionService _subscriptionService;

        public DeadLetterService(
            ITopicService topicService,
            ISubscriptionService subscriptionService)
        {
            _topicService = topicService;
            _subscriptionService = subscriptionService;
        }

        public Subscription UpdateSubscriptionWithDeadLetter(string projectId, string subscriptionId, string deadLetterTopicId, 
            int maximumDeliveryAttempts)
        {
            var subscription = GetSubscription(projectId, subscriptionId);
            var deadletterTopic = GetDeadLetterTopic(projectId, deadLetterTopicId);

            //add the deadletter policy to the subscription
            AddDeadLetterPolicyToSubscription(subscription, deadletterTopic, maximumDeliveryAttempts);

            //update the original subscription with the retry and deadletter changes
            return _subscriptionService.UpdateSubscription(subscription, Subscription.DeadLetterPolicyFieldNumber);
        }

        private Topic GetDeadLetterTopic(string projectId, string deadLetterTopicId)
        {
            if (!_topicService.TopicExists(projectId, deadLetterTopicId))
            {
                throw new Exception();
            }

            //get deadletter topic
            return _topicService.GetTopic(projectId, deadLetterTopicId);
        }

        private Subscription GetSubscription(string projectId, string subscriptionId)
        {
            if (!_subscriptionService.SubscriptionExists(projectId, subscriptionId))
            {
                throw new Exception();
            }

            return _subscriptionService.GetSubscription(projectId, subscriptionId);
        }

        private static void AddDeadLetterPolicyToSubscription(Subscription subscription, Topic deadletterTopic, int maxDeliveryAttempts)
        {
            subscription.DeadLetterPolicy = new DeadLetterPolicy
            {
                DeadLetterTopic = deadletterTopic.Name,
                MaxDeliveryAttempts = maxDeliveryAttempts
            };
        }
    }
}