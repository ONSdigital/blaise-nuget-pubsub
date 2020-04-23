
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
using System.Threading;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class FluentApiSubscriptionTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscriptionId;
        private TestMessageHandler _messageHandler;
        private TestScheduledMessageHandler _scheduledMessageHandler;
        private FluentQueueApi _sut;

        [SetUp]
        public void Setup()
        {
            _projectId = "ons-blaise-dev";
            _topicId = "blaise-nuget-topic";
            _subscriptionId = "blaise-nuget-subscription";
            _messageHandler = new TestMessageHandler();
            _scheduledMessageHandler = new TestScheduledMessageHandler();
            _sut = new FluentQueueApi();

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\Jamie\source\ons-blaise-dev-adb1a24f1dbd.json");
        }

        [Test]
        public void Given_There_Is_One_Message_Available_When_I_Call_Consume_Using_FluentApi_For_One_Message_Then_The_Message_Is_Handled()
        {
            //arrange
            var message = $"Hello, world {Guid.NewGuid()}";
            PublishMessage(message);

            //act
            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscriptionId)
                .Consume(1, _messageHandler)
                .Now();

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(1, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message));
        }

        [Test]
        public void Given_There_Are_Two_Message_Available_When_I_Call_Consume_Using_FluentApi_For_All_Messages_Then_All_Messages_Are_Handled()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";

            PublishMessage(message1);
            PublishMessage(message2);

            //act
            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscriptionId)
                .Consume(10, _messageHandler)
                .Now();

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(2, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message1));
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message2));
        }

        [Test]
        public void Given_There_Is_Two_Messages_Available_When_I_Schedule_Consume_Using_FluentApi_For_One_Message_Every_30_Seconds_Then_Both_Messages_Are_Handled()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";

            PublishMessage(message1);
            PublishMessage(message2);

            //act
            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscriptionId)
                .Consume(1, _scheduledMessageHandler)
                .Every(30, IntervalType.Seconds);

            Thread.Sleep(60000);

            //assert
            Assert.IsNotNull(_scheduledMessageHandler.MessagesHandled);
            Assert.AreEqual(2, _scheduledMessageHandler.MessagesHandled.Count);

            Assert.IsTrue(_scheduledMessageHandler.MessagesHandled.ContainsKey(message1));
            Assert.IsTrue(_scheduledMessageHandler.MessagesHandled.ContainsKey(message2));

            Assert.AreNotEqual(_scheduledMessageHandler.MessagesHandled[message1], _scheduledMessageHandler.MessagesHandled[message2]);
        }

        [Test]
        public void Given_There_Are_Two_Messages_Available_When_I_Schedule_Consume_Using_FluentApi_For_All_Messages_Then_All_Messages_Are_Handled()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";

            PublishMessage(message1);
            PublishMessage(message2);

            //act
            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscriptionId)
                .Consume(10, _scheduledMessageHandler)
                .Every(30, IntervalType.Seconds);

            Thread.Sleep(20000);

            //assert
            Assert.IsNotNull(_scheduledMessageHandler.MessagesHandled);
            Assert.AreEqual(2, _scheduledMessageHandler.MessagesHandled.Count);

            Assert.IsTrue(_scheduledMessageHandler.MessagesHandled.ContainsKey(message1));
            Assert.IsTrue(_scheduledMessageHandler.MessagesHandled.ContainsKey(message2));
        }


        private void PublishMessage(string message)
        {
            var publishService = new PublishService();
            publishService.PublishMessage(_projectId, _topicId, message);
        }
    }
}
