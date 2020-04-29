
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using Google.Cloud.PubSub.V1;
using NUnit.Framework;
using System;
using System.Linq;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class FluentApiPublishTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscriptionId;
        private TestMessageHandler _messageHandler;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;

        private FluentQueueApi _sut;

        public FluentApiPublishTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {  
            _messageHandler = new TestMessageHandler();
            _subscriptionService = new SubscriptionService();
            _topicService = new TopicService();

            _projectId = "ons-blaise-dev";
            _topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            _topicService.CreateTopic(_projectId, _topicId);
            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscriptionId);

            _sut = new FluentQueueApi();
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionService.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _topicId);
        }

        [Test]
        public void Given_A_Message_When_I_Call_PublishMessage_Then_The_Message_Is_Published()
        {
            //arrange
            var message = $"Hello, world {Guid.NewGuid()}";

            //act
            _sut
                .ForProject(_projectId)
                .ForTopic(_topicId)
                .Publish(message);

            var result = GetMessage();

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(message, result);
        }

        private string GetMessage()
        {
            var subscriberService = SubscriberServiceApiClient.Create();
            var subscriptionName = new SubscriptionName(_projectId, _subscriptionId);
            var response = subscriberService.Pull(subscriptionName, returnImmediately: true, maxMessages: 1);

            var receivedMessage = response.ReceivedMessages.FirstOrDefault();
            subscriberService.Acknowledge(subscriptionName, new[] { receivedMessage.AckId });

            return receivedMessage.Message.Data.ToStringUtf8();
        }
    }
}
