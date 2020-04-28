
using Blaise.Nuget.PubSub.Api.Helpers;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Blaise.Nuget.PubSub.Core.Services;
using System;
using System.Collections.Generic;
using Unity;

namespace Blaise.Nuget.PubSub.Api
{
    public sealed class FluentQueueApi : IFluentQueueApi
    {
        private readonly IPublishService _publishService;
        private readonly ISubscriptionService _subscriptionService;

        private string _projectId;
        private string _topicId;
        private string _subscriptionId;

        //This constructor is needed for unit testing but should not be visible from services that ingest the package
        internal FluentQueueApi(
            IPublishService publishService,
            ISubscriptionService subscriptionService)
        {
            _publishService = publishService;
            _subscriptionService = subscriptionService;
        }

        public FluentQueueApi()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<IPublishService, PublishService>();
            unityContainer.RegisterType<ISubscriptionService, SubscriptionService>();

            _publishService = unityContainer.Resolve<IPublishService>();
            _subscriptionService = unityContainer.Resolve<ISubscriptionService>();
        }

        public IFluentQueueApi ForProject(string projectId)
        {
            projectId.ThrowExceptionIfNullOrEmpty("projectId");

            _projectId = projectId;

            return this;
        }

        public IFluentPublishApi ForTopic(string topicId)
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

            _publishService.PublishMessage(_projectId, _topicId, message, attributes);
        }

        public IFluentSubscriptionApi ForSubscription(string subscriptionId)
        {
            subscriptionId.ThrowExceptionIfNullOrEmpty("subscriptionId");

            _subscriptionId = subscriptionId;

            return this;
        }

        public void StartConsuming(IMessageHandler messageHandler)
        {
            ValidateProjectIdIsSet();
            ValidateSubscriptionIdIsSet();

            messageHandler.ThrowExceptionIfNull("messageHandler");

            _subscriptionService.StartConsuming(_projectId, _subscriptionId, messageHandler);
        }

        public void StopConsuming()
        {
            _subscriptionService.StopConsuming();
        }

        private void ValidateProjectIdIsSet()
        {
            if (string.IsNullOrWhiteSpace(_projectId))
            {
                throw new NullReferenceException("The 'ForProject' step needs to be called prior to this");
            }
        }

        private void ValidateTopicIdIsSet()
        {
            if (string.IsNullOrWhiteSpace(_topicId))
            {
                throw new NullReferenceException("The 'ForTopic' step needs to be called prior to this");
            }
        }

        private void ValidateSubscriptionIdIsSet()
        {
            if (string.IsNullOrWhiteSpace(_subscriptionId))
            {
                throw new NullReferenceException("The 'ForSubscription' step needs to be called prior to this");
            }
        }
    }
}
