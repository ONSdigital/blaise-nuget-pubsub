
using Blaise.Nuget.PubSub.Api.Helpers;
using Blaise.Nuget.PubSub.Contracts.Enums;
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
        private readonly ISchedulerService _schedulerService;

        private string _projectId;
        private string _topicId;
        private string _subscriptionId;
        private int _numberOfMessages;
        private IMessageHandler _messageHandler;

        //This constructor is needed for unit testing but should not be visible from services that ingest the package
        internal FluentQueueApi(
            IPublishService publishService,
            ISubscriptionService subscriptionService,
            ISchedulerService schedulerService)
        {
            _publishService = publishService;
            _subscriptionService = subscriptionService;
            _schedulerService = schedulerService;
        }

        public FluentQueueApi()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<IPublishService, PublishService>();
            unityContainer.RegisterType<ISubscriptionService, SubscriptionService>();
            unityContainer.RegisterType<ICronExpressionService, CronExpressionService>();
            unityContainer.RegisterType<ISchedulerService, SchedulerService>();

            _publishService = unityContainer.Resolve<IPublishService>();
            _subscriptionService = unityContainer.Resolve<ISubscriptionService>();
            _schedulerService = unityContainer.Resolve<ISchedulerService>();
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

        public IFluentSubscriptionApi Consume(int numberOfMessages, IMessageHandler messageHandler)
        {
            messageHandler.ThrowExceptionIfNull("messageHandler");

            _numberOfMessages = numberOfMessages;
            _messageHandler = messageHandler;

            return this;
        }

        public void Now()
        {
            ValidateProjectIdIsSet();
            ValidateSubscriptionIdIsSet();
            ValidateMessageHandlerIsSet();

            _subscriptionService.Consume(_projectId, _subscriptionId, _numberOfMessages, _messageHandler);
        }

        public void Every(int intervalNumber, IntervalType intervalType)
        {
            ValidateProjectIdIsSet();
            ValidateSubscriptionIdIsSet();
            ValidateMessageHandlerIsSet();

            _schedulerService.Schedule(
                () => _subscriptionService.Consume(_projectId, _subscriptionId, _numberOfMessages, _messageHandler),
                intervalNumber,
                intervalType);
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

        private void ValidateMessageHandlerIsSet()
        {
            if (_messageHandler == null)
            {
                throw new NullReferenceException("The 'Consume' step needs to be called prior to this");
            }
        }
    }
}
