
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class FluentApiMultipleSubscriptionTests
    {
        private string _projectId;
        private string _topic1Id;
        private string _topic2Id;
        private string _subscription1Id;
        private string _subscription2Id;
        private int _messageTimeoutInSeconds;

        private TestMessageHandler _messageHandler;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;

        private FluentQueueApi _sut;

        public FluentApiMultipleSubscriptionTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {  
            _messageHandler = new TestMessageHandler();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService(_topicService);

            _projectId = "ons-blaise-dev";
            _topic1Id = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _topic2Id = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _subscription1Id = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _subscription2Id = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _messageTimeoutInSeconds = 60;

            _topicService.CreateTopic(_projectId, _topic1Id);
            _topicService.CreateTopic(_projectId, _topic2Id);
            _subscriptionService.CreateSubscription(_projectId, _topic1Id, _subscription1Id, _messageTimeoutInSeconds);
            _subscriptionService.CreateSubscription(_projectId, _topic2Id, _subscription2Id, _messageTimeoutInSeconds);

            _sut = new FluentQueueApi();
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionService.DeleteSubscription(_projectId, _subscription1Id);
            _subscriptionService.DeleteSubscription(_projectId, _subscription2Id);
            _topicService.DeleteTopic(_projectId, _topic1Id);
            _topicService.DeleteTopic(_projectId, _topic2Id);
        }

        [Test]
        public void Given_There_Are_Two_Subscriptions_When_I_Call_StartConsuming_Both_Subsriptions_Are_Handled()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Hello, world {Guid.NewGuid()}";

            //act
            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscription1Id)
                .StartConsuming(_messageHandler);

            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscription2Id)
                .StartConsuming(_messageHandler);

            PublishMessage(_topic1Id, message1);
            PublishMessage(_topic2Id, message2);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            _sut.StopConsuming();

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(2, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message1));
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message2));
        }

        [Test]
        public void Given_I_Use_The_Same_Client_For_Both_Subscriptions_When_I_Call_StopConsuming_Using_FluentApi_Then_Subsequent_Messages_Are_Not_Handled_Only_For_The_Last_Subscription()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";

            //act
            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscription1Id)
                .StartConsuming(_messageHandler);

            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscription2Id)
                .StartConsuming(_messageHandler);

            PublishMessage(_topic1Id, message1);
            PublishMessage(_topic2Id, message2);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            _sut.StopConsuming();

            PublishMessage(_topic1Id, message1);
            PublishMessage(_topic2Id, message2);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(3, _messageHandler.MessagesHandled.Count);
            Assert.AreEqual(2, _messageHandler.MessagesHandled.Count(h => h.Contains(message1)));
            Assert.AreEqual(1, _messageHandler.MessagesHandled.Count(h => h.Contains(message2)));
        }

        private void PublishMessage(string topicId, string message)
        {
            _sut
                .ForProject(_projectId)
                .ForTopic(topicId)
                .Publish(message);
        }
    }
}
