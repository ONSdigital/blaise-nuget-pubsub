
using Blaise.Nuget.PubSub.Api.Helpers;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Blaise.Nuget.PubSub.Core.Services;
using System;
using System.Collections.Generic;
using Blaise.Nuget.PubSub.Contracts.Enums;
using Unity;

namespace Blaise.Nuget.PubSub.Api
{
    public sealed class FluentQueueApi : IFluentQueueApi
    {
        private readonly IPublisherService _publisherService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ITopicService _topicService;
        private readonly ISubscriberService _subscriberService;
        private readonly IDeadLetterService _deadLetterService;
        private readonly IExponentialBackOffService _exponentialBackOffService;
        private readonly IIamPolicyRequestService _iamPolicyRequestService;

        private string _projectId;
        private string _topicId;
        private string _subscriptionId;

        //This constructor is needed for unit testing but should not be visible from services that ingest the package
        internal FluentQueueApi(
            IPublisherService publisherService,
            ISubscriptionService subscriptionService,
            ITopicService topicService,
            ISubscriberService subscriberService,
            IDeadLetterService deadLetterService,
            IExponentialBackOffService exponentialBackOffService,
            IIamPolicyRequestService iamPolicyRequestService)
        {
            _publisherService = publisherService;
            _subscriptionService = subscriptionService;
            _topicService = topicService;
            _subscriberService = subscriberService;
            _deadLetterService = deadLetterService;
            _exponentialBackOffService = exponentialBackOffService;
            _iamPolicyRequestService = iamPolicyRequestService;
        }

        public FluentQueueApi()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<IPublisherService, PublisherService>();
            unityContainer.RegisterType<ISubscriptionService, SubscriptionService>();
            unityContainer.RegisterType<ITopicService, TopicService>();
            unityContainer.RegisterSingleton<ISubscriberService, SubscriberService>();
            unityContainer.RegisterSingleton<IDeadLetterService, DeadLetterService>();
            unityContainer.RegisterSingleton<IExponentialBackOffService, ExponentialBackOffService>();
            unityContainer.RegisterSingleton<IIamPolicyRequestService, IamPolicyRequestService>();

            _publisherService = unityContainer.Resolve<IPublisherService>();
            _subscriptionService = unityContainer.Resolve<ISubscriptionService>();
            _topicService = unityContainer.Resolve<ITopicService>();
            _subscriberService = unityContainer.Resolve<ISubscriberService>();
            _deadLetterService = unityContainer.Resolve<IDeadLetterService>();
            _exponentialBackOffService = unityContainer.Resolve<IExponentialBackOffService>();
            _iamPolicyRequestService = unityContainer.Resolve<IIamPolicyRequestService>();
        }

        public IFluentQueueApi WithProject(string projectId)
        {
            projectId.ThrowExceptionIfNullOrEmpty("projectId");

            _projectId = projectId;

            return this;
        }

        public IFluentQueueApi CreateTopic(string topicId)
        {
            topicId.ThrowExceptionIfNullOrEmpty("topicId");
            ValidateProjectIdIsSet();

            _topicService.CreateTopic(_projectId, topicId);

            _topicId = topicId;

            return this;
        }

        public IFluentQueueApi CreateSubscription(string subscriptionId, int ackTimeoutInSeconds = 600)
        {
            subscriptionId.ThrowExceptionIfNullOrEmpty("subscriptionId");
            ValidateAckTimeoutIsInRange(ackTimeoutInSeconds);

            ValidateProjectIdIsSet();
            ValidateTopicIdIsSet();

            _subscriptionService.CreateSubscription(_projectId, _topicId, subscriptionId, ackTimeoutInSeconds);
            _subscriptionId = subscriptionId;

            return this;
        }

        public IFluentQueueApi WithExponentialBackOff(int minimumBackOffInSeconds = 10, int maximumBackOffInSeconds = 600)
        {
            ValidateMinimumBackOff(minimumBackOffInSeconds);
            ValidateMaximumBackOff(maximumBackOffInSeconds);

            ValidateProjectIdIsSet();
            ValidateSubscriptionIdIsSet();

            _exponentialBackOffService.UpdateSubscriptionWithExponentialBackOff(_projectId, _subscriptionId,
                minimumBackOffInSeconds,
                maximumBackOffInSeconds);

            return this;
        }

        public IFluentQueueApi WithDeadLetter(string serviceAccountName, string deadLetterTopicId, int maximumDeliveryAttempts = 5)
        {
            serviceAccountName.ThrowExceptionIfNullOrEmpty("serviceAccountName");
            deadLetterTopicId.ThrowExceptionIfNullOrEmpty("deadLetterTopicId");
            ValidateMaximumDeliveryAttempts(maximumDeliveryAttempts);
            
            ValidateProjectIdIsSet();
            ValidateSubscriptionIdIsSet();

            var subscription = _deadLetterService.UpdateSubscriptionWithDeadLetter(_projectId, _subscriptionId, deadLetterTopicId,
                maximumDeliveryAttempts);

            _iamPolicyRequestService.GrantPermissionsForAccount(subscription.Name, serviceAccountName, IamRoleType.Subscriber);

            return this;
        }

        public IFluentQueueApi WithTopic(string topicId)
        {
            topicId.ThrowExceptionIfNullOrEmpty("topicId");

            _topicId = topicId;

            return this;
        }

        public void Publish(string message, Dictionary<string, string> attributes = null)
        {
            message.ThrowExceptionIfNullOrEmpty("message");

            ValidateProjectIdIsSet();
            ValidateTopicIdIsSet();

            _publisherService.PublishMessage(_projectId, _topicId, message, attributes);
        }

        public IFluentQueueApi WithSubscription(string subscriptionId)
        {
            subscriptionId.ThrowExceptionIfNullOrEmpty("subscriptionId");

            _subscriptionId = subscriptionId;

            return this;
        }

        public void StartConsuming(IMessageHandler messageHandler, bool throttle = false)
        {
            ValidateProjectIdIsSet();
            ValidateSubscriptionIdIsSet();

            messageHandler.ThrowExceptionIfNull("messageHandler");

            _subscriberService.StartConsuming(_projectId, _subscriptionId, messageHandler, throttle);
        }

        public void StopConsuming()
        {
            _subscriberService.StopConsuming();
        }

        private void ValidateProjectIdIsSet()
        {
            if (string.IsNullOrWhiteSpace(_projectId))
            {
                throw new NullReferenceException("The 'WithProject' step needs to be called prior to this");
            }
        }

        private void ValidateTopicIdIsSet()
        {
            if (string.IsNullOrWhiteSpace(_topicId))
            {
                throw new NullReferenceException("The 'WithTopic' or 'CreateTopic' step needs to be called prior to this");
            }
        }

        private void ValidateSubscriptionIdIsSet()
        {
            if (string.IsNullOrWhiteSpace(_subscriptionId))
            {
                throw new NullReferenceException("The 'WithSubscription' or 'CreateSubscription' step needs to be called prior to this");
            }
        }

        private static void ValidateAckTimeoutIsInRange(int ackTimeoutInSeconds)
        {
            if (ackTimeoutInSeconds < 10 || ackTimeoutInSeconds > 600)
            {
                throw new ArgumentOutOfRangeException($"The deadline for acking messages must be between the values '1' and '600'");
            }
        }

        private static void ValidateMinimumBackOff(int minimumBackOffInSeconds)
        {
            if (minimumBackOffInSeconds < 10 || minimumBackOffInSeconds > 600)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumBackOffInSeconds), 
                    "The range for the minimum back off between retries is between '10' and '600'");
            }
        }

        private static void ValidateMaximumBackOff(int maximumBackOffInSeconds)
        {
            if (maximumBackOffInSeconds < 10 || maximumBackOffInSeconds > 600)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumBackOffInSeconds), 
                    "The range for the minimum back off between retries is between '10' and '600'");
            }
        }

        private void ValidateMaximumDeliveryAttempts(int maximumDeliveryAttempts)
        {
            if (maximumDeliveryAttempts < 5 || maximumDeliveryAttempts > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumDeliveryAttempts), 
                    "The range for the maximum number of delivery attempts is between '5' and '100'");
            }
        }
    }
}
