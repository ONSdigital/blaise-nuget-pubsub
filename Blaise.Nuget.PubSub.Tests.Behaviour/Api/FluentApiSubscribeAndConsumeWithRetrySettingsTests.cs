﻿using System;
using System.Threading;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Core.Models;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Api
{
    public class FluentApiSubscribeAndConsumeWithRetrySettingsTests
    {
        private int _ackTimeoutInSeconds;

        private string _projectId;
        private string _topicId;
        private string _subscriptionId;

        private TestMessageHandler _messageHandler;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;

        private FluentQueueApi _sut;

        public FluentApiSubscribeAndConsumeWithRetrySettingsTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _messageHandler = new TestMessageHandler();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService(new DeadLetterService(_topicService));

            var configurationHelper = new ConfigurationHelper();
            _projectId = configurationHelper.ProjectId;
            _topicId = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            _ackTimeoutInSeconds = 60;
            var connectionSettings = new SubscriptionSettingsModel { AckTimeoutInSeconds = _ackTimeoutInSeconds };

            _topicService.CreateTopic(_projectId, _topicId);
            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscriptionId, connectionSettings);

            _sut = new FluentQueueApi();
        }

        [TearDown]
        public void TearDown()
        {
            _sut.StopConsuming();
            _subscriptionService.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _topicId);
        }

        [Test]
        public void Given_I_Supply_Valid_Arguments_I_Can_Create_And_Consume_From_A_Subscription_In_One_Call()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";

            //act
            _sut
                .WithProject(_projectId)
                .WithTopic(_topicId)
                .CreateSubscription(_subscriptionId, 600)
                .StartConsuming(_messageHandler, true);

            PublishMessage(message1);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(1, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message1));
        }

        private void PublishMessage(string message)
        {
            _sut
                .WithProject(_projectId)
                .WithTopic(_topicId)
                .Publish(message);
        }
    }
}
