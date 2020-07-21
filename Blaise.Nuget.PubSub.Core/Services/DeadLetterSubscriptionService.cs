﻿using System.Collections.Generic;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Blaise.Nuget.PubSub.Core.Models;
using Google.Cloud.Iam.V1;
using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;

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

        public Subscription CreateSubscriptionWithDeadLetter(string projectId, string topicId, string subscriptionId,
            int ackTimeoutInSeconds, RetrySettingsModel settingsModel)
        {
            //create subscription
            var subscription = _subscriptionService.CreateSubscription(projectId, topicId, subscriptionId, ackTimeoutInSeconds);

            //create deadletter topic and subscription
            var deadletterTopic = CreateDeadLetterTopicAndSubscription(projectId, topicId, subscriptionId, ackTimeoutInSeconds);
            
            //grant permissions for the deadletter topic and subscription
            GrantSubscriberPermissionsForSubscription(subscription, settingsModel.ServiceAccountName);
            GrantPublishPermissionsForTopic(deadletterTopic, settingsModel.ServiceAccountName);

            //add the exponential back off policy to the subscription
            AddRetrySettingsToSubscription(subscription, settingsModel.MinimumBackOffInSeconds,
                settingsModel.MaximumBackOffInSeconds);

            //add the deadletter policy to the subscription
            AddDeadLetterPolicyToSubscription(subscription, deadletterTopic, settingsModel.MaximumDeliveryAttempts);

            //update the original subscription with the retry and deadletter changes
            return UpdateSubscription(subscription);
        }

        private Topic CreateDeadLetterTopicAndSubscription(string projectId, string topicId, string subscriptionId, int ackTimeoutInSeconds)
        {
            //create dead letter topic
            var deadletterTopicId = $"{topicId}-deadletter";
            var topic = _topicService.CreateTopic(projectId, deadletterTopicId);

            //create dead letter subscription
            var deadletterSubscriptionId = $"{subscriptionId}-deadletter";
            _subscriptionService.CreateSubscription(projectId, deadletterTopicId, deadletterSubscriptionId, ackTimeoutInSeconds);

            return topic;
        }

        private static void AddRetrySettingsToSubscription(Subscription subscription, int minimumBackOffInSeconds, int maximumBackOffInSeconds)
        {
            subscription.RetryPolicy = new RetryPolicy
            {
                MinimumBackoff = new Duration { Seconds = minimumBackOffInSeconds },
                MaximumBackoff = new Duration { Seconds = maximumBackOffInSeconds }
            };
        }

        private static void AddDeadLetterPolicyToSubscription(Subscription subscription, Topic deadletterTopic, int maxDeliveryAttempts)
        {
            subscription.DeadLetterPolicy = new DeadLetterPolicy
            {
                DeadLetterTopic = deadletterTopic.Name,
                MaxDeliveryAttempts = maxDeliveryAttempts
            };
        }

        private static Subscription UpdateSubscription(Subscription subscription)
        {
            var subscriberServiceClient = SubscriberServiceApiClient.Create();

            return subscriberServiceClient.UpdateSubscription(
                new UpdateSubscriptionRequest
                {
                    Subscription = subscription,
                    UpdateMask = FieldMask.FromFieldNumbers<Subscription>(new List<int>
                    {
                        Subscription.RetryPolicyFieldNumber,
                        Subscription.DeadLetterPolicyFieldNumber
                        
                    })
                });
        }

        private static void GrantSubscriberPermissionsForSubscription(Subscription subscription,
            string serviceAccountName)
        {
            var publisherServiceClient = PublisherServiceApiClient.Create();

            var subscriptionRequest = new SetIamPolicyRequest
            {
                Resource = subscription.Name,
                Policy = new Policy
                {
                    Bindings =
                    {
                        new Binding {
                            Role = "roles/pubsub.subscriber",
                            Members = { $"serviceAccount:{serviceAccountName}@gcp-sa-pubsub.iam.gserviceaccount.com" } }
                    }
                }
            };

            publisherServiceClient.SetIamPolicy(subscriptionRequest);
        }

        private static void GrantPublishPermissionsForTopic(Topic topic, string serviceAccountName)
        {
            var publisherServiceClient = PublisherServiceApiClient.Create();

            var topicRequest = new SetIamPolicyRequest
            {
                Resource = topic.Name,
                Policy = new Policy
                {
                    Bindings =
                    {
                        new Binding {
                            Role = "roles/pubsub.publisher",
                            Members = { $"serviceAccount:{serviceAccountName}@gcp-sa-pubsub.iam.gserviceaccount.com" } }
                    }
                }
            };

            publisherServiceClient.SetIamPolicy(topicRequest);
        }
    }
}