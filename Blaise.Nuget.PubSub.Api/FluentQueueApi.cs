
using Blaise.Nuget.PubSub.Api.Helpers;
using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Blaise.Nuget.PubSub.Core.Models;
using System;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Api
{
    public sealed class FluentQueueApi : IFluentQueueApi
    {
        private readonly IPublisherService _publisherService;
        private readonly ISubscriptionService _subscriptionService;

        private string _projectId;
        private string _topicId;
        private string _subscriptionId;
        private int _numberOfMessages;
        private IMessageHandler _messageHandler;

        public FluentQueueApi(
            IPublisherService publisherService,
            ISubscriptionService subscriptionService)
        {
            _publisherService = publisherService;
            _subscriptionService = subscriptionService;
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

            _publisherService.PublishMessage(_projectId, _topicId, message, attributes);
        }

        public IFluentSubscriptionApi ForSubscription(string subscriptionId)
        {
            _subscriptionId = subscriptionId;

            return this;
        }

        public IFluentSubscriptionApi Pull(int numberOfMessages, IMessageHandler messageHandler)
        {
            _numberOfMessages = numberOfMessages;
            _messageHandler = messageHandler;

            return this;
        }

        public void Every(int intervalNumber, IntervalType intervalType)
        {
            var scheduleModel = new ScheduleModel
            {
                NumberOfMessages = _numberOfMessages,
                IntervalNumber = intervalNumber,
                IntervalType = intervalType
            };

            _subscriptionService.Subscribe(_projectId, _subscriptionId, _messageHandler, scheduleModel);
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
    }
}
