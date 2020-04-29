﻿using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
using System.Threading;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class SubscriberServiceTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscriptionId;

        private TestMessageHandler _messageHandler;
        private TopicService _topicService;
        private SubscriptionService _subscriptionService;
        private PublisherService _publisherService;

        private SubscriberService _sut;

        public SubscriberServiceTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _projectId = "ons-blaise-dev";
            _topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            _messageHandler = new TestMessageHandler();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService();
            _publisherService = new PublisherService();

            _topicService.CreateTopic(_projectId, _topicId);
            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscriptionId);

            _sut = new SubscriberService();
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionService.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _topicId);
        }

        [Test]
        public void Given_Three_Messages_Are_Available_When_I_Call_StartConsuming_Then_The_Three_Messages_Are_Processed()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";
            var message3 = $"Yo, Yo {Guid.NewGuid()}";

            PublishMessage(message1);
            PublishMessage(message2);
            PublishMessage(message3);

            //act
            _sut.StartConsuming(_projectId, _subscriptionId, _messageHandler);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            _sut.StopConsuming();

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(3, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message1));
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message2));
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message3));
        }

        [Test]
        public void Given_No_Subscriptions_When_I_Call_StopConsuming_Then_InvalidOperationException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<InvalidOperationException>(() => _sut.StopConsuming());
            Assert.AreEqual("No subscriptons have been setup", exception.Message);

        }
     
        private void PublishMessage(string message)
        {
            _publisherService.PublishMessage(_projectId, _topicId, message);
        }
    }
}