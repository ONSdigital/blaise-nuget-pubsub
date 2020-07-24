using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.MultipleSubscriptions
{
    public class PublishToSingleTopicWithMultipleSubscriptionsTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscription1Id;
        private string _subscription2Id;
        private int _ackTimeoutInSeconds;

        private MessageHelper _messageHelper;
        private TopicService _topicService;
        private SubscriptionService _subscriptionService;

        private PublisherService _sut;

        public PublishToSingleTopicWithMultipleSubscriptionsTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            var configurationHelper = new ConfigurationHelper();
            _projectId = configurationHelper.ProjectId;
            _topicId = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
            _subscription1Id = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";
            _subscription2Id = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            _ackTimeoutInSeconds = 60;

            _messageHelper = new MessageHelper();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService();

            _topicService.CreateTopic(_projectId, _topicId);
            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscription1Id, _ackTimeoutInSeconds);
            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscription2Id, _ackTimeoutInSeconds);

            _sut = new PublisherService();
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionService.DeleteSubscription(_projectId, _subscription1Id);
            _subscriptionService.DeleteSubscription(_projectId, _subscription2Id);
            _topicService.DeleteTopic(_projectId, _topicId);
        }

        [Test]
        public void Given_A_Message_When_I_Call_PublishMessage_Then_The_Message_Is_Published()
        {
            //arrange
            var message = $"Hello, world {Guid.NewGuid()}";

            //act
            _sut.PublishMessage(_projectId, _topicId, message);
            var result1 = _messageHelper.GetMessage(_projectId, _subscription1Id);
            var result2 = _messageHelper.GetMessage(_projectId, _subscription2Id);

            //assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.AreEqual(message, result1);
            Assert.AreEqual(message, result2);
        }
    }
}
