using System;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Core.Models;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Api
{
    public class FluentApiPublishTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscriptionId;
        private int _ackTimeoutInSeconds;
        private SubscriptionSettingsModel _settingsModel;

        private MessageHelper _messageHelper;
        private TopicService _topicService;
        private SubscriptionService _subscriptionService;

        private FluentQueueApi _sut;

        public FluentApiPublishTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _messageHelper = new MessageHelper();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService(new DeadLetterService(_topicService));
            
            var configurationHelper = new ConfigurationHelper();
            _projectId = configurationHelper.ProjectId;
            _topicId = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";
            
            _ackTimeoutInSeconds = 60;

            _settingsModel = new SubscriptionSettingsModel { AckTimeoutInSeconds = _ackTimeoutInSeconds };
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

            _topicService.CreateTopic(_projectId, _topicId);
            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscriptionId, _settingsModel);

            //act
            _sut
                .WithProject(_projectId)
                .WithTopic(_topicId)
                .Publish(message);

            var result = _messageHelper.GetMessage(_projectId, _subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(message, result);
        }

        [Test]
        public void Given_A_Message_When_I_Call_PublishMessage_Using_CreateTopic_Then_The_Message_Is_Published()
        {
            //arrange
            var message = $"Hello, world {Guid.NewGuid()}";

            _topicService.CreateTopic(_projectId, _topicId);            

            //act
            _sut
                .WithProject(_projectId)
                .CreateTopic(_topicId)
                .CreateSubscription(_subscriptionId, _ackTimeoutInSeconds)
                .Publish(message);

            var result = _messageHelper.GetMessage(_projectId, _subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(message, result);
        }
    }
}
