using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.Iam.V1;
using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class DeadLetterSubscriptionService : IDeadLetterSubscriptionService
    {
        private readonly ITopicService _topicService;
        private readonly ISubscriptionService _subscriptionService;

        public DeadLetterSubscriptionService(
            ITopicService topicService,
            ISubscriptionService subscriptionService)
        {
            _topicService = topicService;
            _subscriptionService = subscriptionService;
        }

        public Subscription CreateSubscriptionWithDeadLetter(string projectId, string topicId, string subscriptionId, int messageTimeoutInSeconds, int maxDeliveryAttempts)
        {
            if (maxDeliveryAttempts < 5 || maxDeliveryAttempts > 100)
            {
                throw new ArgumentOutOfRangeException("The maximum number of retries for processing messages must be between '5' and '100'");
            }

            var subscription = _subscriptionService.CreateSubscription(projectId, topicId, subscriptionId, messageTimeoutInSeconds);
            GrantSubscriberPermissionsForSubscription(subscription);

            var deadletterTopic = CreateDeadLetterTopic(projectId, topicId, subscriptionId, messageTimeoutInSeconds);
            GrantPublishPermissionsForTopic(deadletterTopic);

            return UpdateSubscriptionWithDeadLetterPolicy(subscription, deadletterTopic, maxDeliveryAttempts);
        }

        private void GrantSubscriberPermissionsForSubscription(Subscription subscription)
        {
            var publisherServiceClient = PublisherServiceApiClient.Create();

            SetIamPolicyRequest subscriptionRequest = new SetIamPolicyRequest
            {
                Resource = subscription.Name,
                Policy = new Policy
                {
                    Bindings =
                    {
                        new Binding {
                            Role = "roles/pubsub.subscriber",
                            Members = { "serviceAccount:pubsub-ons-blaise-dev@ons-blaise-dev.iam.gserviceaccount.com" } }
                    }
                }
            };

            publisherServiceClient.SetIamPolicy(subscriptionRequest);
        }

        private void GrantPublishPermissionsForTopic(Topic topic)
        {
            var publisherServiceClient = PublisherServiceApiClient.Create();

            SetIamPolicyRequest topicRequest = new SetIamPolicyRequest
            {
                Resource = topic.Name,
                Policy = new Policy
                {
                    Bindings =
                    {
                        new Binding {
                            Role = "roles/pubsub.publisher",
                            Members = { "serviceAccount:pubsub-ons-blaise-dev@ons-blaise-dev.iam.gserviceaccount.com" } }
                    }
                }
            };

            publisherServiceClient.SetIamPolicy(topicRequest);
        }

        private Topic CreateDeadLetterTopic(string projectId, string topicId, string subscriptionId, int messageTimeoutInSeconds)
        {
            //create dead letter topic
            var deadletterTopicId = $"{topicId}-deadletter";
            var topic = _topicService.CreateTopic(projectId, deadletterTopicId);

            //create dead letter subscription
            var deadletterSubscriptionId = $"{subscriptionId}-deadletter";
            _subscriptionService.CreateSubscription(projectId, deadletterTopicId, deadletterSubscriptionId, messageTimeoutInSeconds);

            return topic;
        }

        private Subscription UpdateSubscriptionWithDeadLetterPolicy(Subscription subscription, Topic deadletterTopic, int maxDeliveryAttempts)
        {
            subscription.DeadLetterPolicy = new DeadLetterPolicy
            {
                DeadLetterTopic = deadletterTopic.Name,
                MaxDeliveryAttempts = maxDeliveryAttempts
            };

            var subscriberServiceClient = SubscriberServiceApiClient.Create();

            return subscriberServiceClient.UpdateSubscription(
                new UpdateSubscriptionRequest
                {
                    Subscription = subscription,
                    UpdateMask = FieldMask.FromFieldNumbers<Subscription>(new List<int> { 13 })
                });
        }
    }
}


