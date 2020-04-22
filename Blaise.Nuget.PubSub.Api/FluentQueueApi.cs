
using Blaise.Nuget.PubSub.Api.Interfaces;
using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Api
{
    public sealed class FluentQueueApi : IFluentQueueApi
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IPublisherService _publisherService;
        private readonly ISubscriptionService _subscriptionService;

        private string _projectId;
        private string _topicId;

        public FluentQueueApi(
            IConfigurationProvider configurationProvider,
            IPublisherService publisherService,
            ISubscriptionService subscriptionService)
        {
            _configurationProvider = configurationProvider;
            _publisherService = publisherService;
            _subscriptionService = subscriptionService;
        }

        public IFluentQueueApi ForProject(string projectId)
        {
            _projectId = projectId;

            return this;
        }

        public IFluentPublishApi ForTopic(string topicId)
        {
            _topicId = topicId;

            return this;
        }

        public void Publish(string message, Dictionary<string, string> attributes = null)
        {
            _publisherService.PublishMessage(_projectId, _topicId, message, attributes);
        }

        public IFluentSubscriptionApi ForSubscription(string subscriptionId)
        {
            throw new System.NotImplementedException();
        }

        public IFluentSubscriptionApi Pull(int numberOfMessages, IMessageHandler messageHandler)
        {
            throw new System.NotImplementedException();
        }

        public void Every(int intervalNumber, IntervalType intervalType)
        {
            throw new System.NotImplementedException();
        }
    }
}
