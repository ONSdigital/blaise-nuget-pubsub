using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
using System.Threading;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class SubscriberServiceDeadLetterTests
    {
        private string _projectId;
        private string _topicId;
        private string _deadLetterTopicId;
        private string _subscriptionId;
        private string _deadLetterSubscriptionId;

        private TestMessageHandler _messageHandler;
        private TopicService _topicService;
        private SubscriptionService _subscriptionService;
        private DeadLetterSubscriptionService _deadLetterSubscriptionService;
        private PublisherService _publisherService;

        private SubscriberService _sut;

        public SubscriberServiceDeadLetterTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _projectId = "ons-blaise-dev";
            _topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _deadLetterTopicId = $"{_topicId}-deadletter";
            _subscriptionId = $"blaise-nuget-subscription-{Guid.NewGuid()}";
            _deadLetterSubscriptionId = $"{_subscriptionId}-deadletter";

            _messageHandler = new TestMessageHandler();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService();
            _publisherService = new PublisherService();
            _deadLetterSubscriptionService = new DeadLetterSubscriptionService(_topicService, _subscriptionService);
            _sut = new SubscriberService();
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionService.DeleteSubscription(_projectId, _deadLetterSubscriptionId);
            _subscriptionService.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _deadLetterTopicId);
            _topicService.DeleteTopic(_projectId, _topicId);
        }

        [Test]
        public void Given_DeadLetterSubscription_When_The_Maximum_Number_Of_Retries_Are_Reached_Then_The_Three_Message_Is_Moved_To_The_DeadLetter_Queue()
        {
            //arrange
            var message = $"Hello, world {Guid.NewGuid()}";
            var messageTimeoutInSeconds = 10;
            var maxNumberOfRetries = 5;

            _topicService.CreateTopic(_projectId, _topicId);
            _deadLetterSubscriptionService.CreateSubscriptionWithDeadLetter(_projectId, _topicId, _subscriptionId, messageTimeoutInSeconds, maxNumberOfRetries);

            PublishMessage(message);

            _messageHandler.SetResult(false);

            //act
            _sut.StartConsuming(_projectId, _subscriptionId, _messageHandler);

            Thread.Sleep(20000); // allow time for processing the messages off the queue

            _sut.StopConsuming();

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(maxNumberOfRetries, _messageHandler.MessagesHandled.Count);
        }

        private void PublishMessage(string message)
        {
            _publisherService.PublishMessage(_projectId, _topicId, message);
        }
    }
}
