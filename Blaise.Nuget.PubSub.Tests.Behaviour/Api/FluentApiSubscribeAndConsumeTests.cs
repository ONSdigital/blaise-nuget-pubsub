using System;
using System.Threading;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Core.Models;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Api
{
    public class FluentApiSubscribeAndConsumeTests
    {
        private int _ackTimeoutInSeconds;

        private string _projectId;
        private string _topicId;
        private string _subscriptionId;

        private TestMessageHandler _messageHandler;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;

        private FluentQueueApi _sut;

        public FluentApiSubscribeAndConsumeTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _messageHandler = new TestMessageHandler();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService(new DeadLetterService(_topicService));

            _projectId = "ons-blaise-dev";
            _topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _subscriptionId = $"blaise-nuget-subscription-{Guid.NewGuid()}";
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

        [Test]
        public void Given_There_Are_Three_Available_And_I_Do_Not_Supply_Throttle_When_I_Call_StartConsuming_Using_FluentApi_Then_The_Three_Messages_Are_Processed()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";
            var message3 = $"Yo, Yo {Guid.NewGuid()}";


            //act
            _sut
                .WithProject(_projectId)
                .WithSubscription(_subscriptionId)
                .StartConsuming(_messageHandler);

            PublishMessage(message1);
            PublishMessage(message2);
            PublishMessage(message3);

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
        public void Given_I_Throttle_When_I_Call_StartConsuming_Using_FluentApi_Then_The_Messages_Are_Processed_One_At_A_Time()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";
            var message3 = $"Yo, Yo {Guid.NewGuid()}";

            var concurrencyHandler = new TestConcurrencyMessageHandler(2);

            //act
            _sut
                .WithProject(_projectId)
                .WithSubscription(_subscriptionId)
                .StartConsuming(concurrencyHandler, true);

            PublishMessage(message1);
            PublishMessage(message2);
            PublishMessage(message3);

            Thread.Sleep(10000); // allow time for processing the messages off the queue

            _sut.StopConsuming();

            //assert
            Assert.IsNotNull(concurrencyHandler.MessagesHandled);
            Assert.AreEqual(3, concurrencyHandler.MessagesHandled.Count);
            Assert.IsTrue(concurrencyHandler.MessagesHandled.Contains(message1));
            Assert.IsTrue(concurrencyHandler.MessagesHandled.Contains(message2));
            Assert.IsTrue(concurrencyHandler.MessagesHandled.Contains(message3));
            Assert.IsFalse(concurrencyHandler.HandledConcurrently());
        }

        [Test]
        public void Given_I_Do_Not_Throttle_When_I_Call_StartConsuming_Using_FluentApi_Then_The_Messages_Are_Processed_Concurrently()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";
            var message3 = $"Yo, Yo {Guid.NewGuid()}";

            var concurrencyHandler = new TestConcurrencyMessageHandler(2);

            //act
            _sut
                .WithProject(_projectId)
                .WithSubscription(_subscriptionId)
                .StartConsuming(concurrencyHandler, false);

            PublishMessage(message1);
            PublishMessage(message2);
            PublishMessage(message3);

            Thread.Sleep(10000); // allow time for processing the messages off the queue

            _sut.StopConsuming();

            //assert
            Assert.IsNotNull(concurrencyHandler.MessagesHandled);
            Assert.AreEqual(3, concurrencyHandler.MessagesHandled.Count);
            Assert.IsTrue(concurrencyHandler.MessagesHandled.Contains(message1));
            Assert.IsTrue(concurrencyHandler.MessagesHandled.Contains(message2));
            Assert.IsTrue(concurrencyHandler.MessagesHandled.Contains(message3));
            Assert.IsTrue(concurrencyHandler.HandledConcurrently());
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
