
using Blaise.Nuget.PubSub.Api;
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
        private int _ackDeadlineInSeconds;

        private TestMessageHandler _messageHandler;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;

        private FluentQueueApi _sut;

        public FluentApiSubscriptionTests()
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
            _ackDeadlineInSeconds = 60;

            _topicService.CreateTopic(_projectId, _topicId);
            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackDeadlineInSeconds);

            _sut = new FluentQueueApi();
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionService.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _topicId);
        }

        [Test]
        public void Given_There_Is_One_Message_Available_When_I_Call_StartConsuming_Using_FluentApi_For_One_Message_Then_The_Message_Is_Handled()
        {
            //arrange
            var message = $"Hello, world {Guid.NewGuid()}";

            //act
            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscriptionId)
                .StartConsuming(_messageHandler);

            PublishMessage(message);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            _sut.StopConsuming();

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(1, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message));
        }

        [Test]
        public void Given_There_Are_Two_Message_Available_When_I_Call_StartConsuming_Using_FluentApi_For_All_Messages_Then_All_Messages_Are_Handled()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";

            //act
            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscriptionId)
                .StartConsuming(_messageHandler);


            PublishMessage(message1);
            PublishMessage(message2);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            _sut.StopConsuming();

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(2, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message1));
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message2));
        }

        [Test]
        public void Given_Subscribe_To_One_Message_When_I_Call_StopConsuming_Using_FluentApi_Then_Subsequent_Messages_Are_Not_Handled()
        {
            //arrange
            var message = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";

            //act
            _sut
                .ForProject(_projectId)
                .ForSubscription(_subscriptionId)
                .StartConsuming(_messageHandler);

            PublishMessage(message);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            _sut.StopConsuming();

            PublishMessage(message2);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(1, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message));
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
            _sut
                .ForProject(_projectId)
                .ForTopic(_topicId)
                .Publish(message);
        }
    }
}
