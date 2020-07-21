using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class PublisherServiceTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscriptionId;
        private int _ackTimeoutInSeconds;

        private MessageHelper _messageHelper;
        private TopicService _topicService;
        private SubscriptionService _subscriptionService;

        private PublisherService _sut;

        public PublisherServiceTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            var configurationHelper = new ConfigurationHelper();
            _projectId = configurationHelper.ProjectId;
            _topicId = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            _ackTimeoutInSeconds = 60;

            _messageHelper = new MessageHelper();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService();

            _topicService.CreateTopic(_projectId, _topicId);
            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackTimeoutInSeconds);

            _sut = new PublisherService();
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
            _sut.PublishMessage(_projectId, _topicId, message);
            var result = _messageHelper.GetMessage(_projectId, _subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(message, result);
        }

        [Test]
        public void Given_A_Message_With_Attributes_When_I_Call_PublishMessage_Then_The_Message_With_Attributes_Is_Published()
        {
            //arrange
            var message = $"Hello, world {Guid.NewGuid()}";
            var messageAttributes = new Dictionary<string, string> {{"Header", "TestMessage"}};

            //act
            _sut.PublishMessage(_projectId, _topicId, message, messageAttributes);
            var result = _messageHelper.GetMessageAttributes(_projectId, _subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(messageAttributes["Header"], result["Header"]);
        }
    }
}
