using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
using System.Threading;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.MultipleSubscriptions
{
    public class MultipleSubscriberToASingleTopicTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscription1Id;
        private string _subscription2Id;
        private int _ackTimeoutInSeconds;

        private TopicService _topicService;
        private SubscriptionService _subscriptionService;
        private PublisherService _publisherService;

        public MultipleSubscriberToASingleTopicTests()
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

            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService();
            _publisherService = new PublisherService();

            _topicService.CreateTopic(_projectId, _topicId);

            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscription1Id, _ackTimeoutInSeconds);
            _subscriptionService.CreateSubscription(_projectId, _topicId, _subscription2Id, _ackTimeoutInSeconds);
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionService.DeleteSubscription(_projectId, _subscription1Id);
            _subscriptionService.DeleteSubscription(_projectId, _subscription2Id);
            _topicService.DeleteTopic(_projectId, _topicId);
        }

        [Test]
        public void Given_Two_Subscriptions_To_A_Topic_When_Messages_Are_Published_Both_Subscriptions_Should_Get_The_Messages()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";
            var message3 = $"Yo, Yo {Guid.NewGuid()}";

            var messageHandler1 = new TestMessageHandler();
            var messageHandler2 = new TestMessageHandler();

            var sut1 = new SubscriberService();
            var sut2 = new SubscriberService();

            //act
            sut1.StartConsuming(_projectId, _subscription1Id, messageHandler1);
            sut2.StartConsuming(_projectId, _subscription2Id, messageHandler2);

            PublishMessage(message1);
            PublishMessage(message2);
            PublishMessage(message3);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            sut1.StopConsuming();
            sut2.StopConsuming();

            //assert
            Assert.IsNotNull(messageHandler1.MessagesHandled);
            Assert.IsNotNull(messageHandler2.MessagesHandled);

            Assert.AreEqual(3, messageHandler1.MessagesHandled.Count);
            Assert.AreEqual(3, messageHandler2.MessagesHandled.Count);

            Assert.IsTrue(messageHandler1.MessagesHandled.Contains(message1));
            Assert.IsTrue(messageHandler1.MessagesHandled.Contains(message2));
            Assert.IsTrue(messageHandler1.MessagesHandled.Contains(message3));

            Assert.IsTrue(messageHandler2.MessagesHandled.Contains(message1));
            Assert.IsTrue(messageHandler2.MessagesHandled.Contains(message2));
            Assert.IsTrue(messageHandler2.MessagesHandled.Contains(message3));
        }
     
        private void PublishMessage(string message)
        {
            _publisherService.PublishMessage(_projectId, _topicId, message);
        }
    }
}
