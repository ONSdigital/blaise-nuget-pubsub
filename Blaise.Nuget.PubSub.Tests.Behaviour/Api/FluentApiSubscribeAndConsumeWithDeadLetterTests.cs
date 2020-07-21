using System;
using System.Threading;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Api
{
    public class FluentApiSubscribeAndConsumeWithDeadLetterTests
    {
        private string _projectId;
        private string _topicId;
        private string _deadLetterTopicId;
        private string _subscriptionId;
        private string _deadLetterSubscriptionId;
        private string _serviceAccountName;

        private TestMessageHandler _messageHandler;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;
        private MessageHelper _messageHelper;

        private FluentQueueApi _sut;

        public FluentApiSubscribeAndConsumeWithDeadLetterTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _messageHandler = new TestMessageHandler();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService();
            _messageHelper = new MessageHelper();

            var configurationHelper = new ConfigurationHelper();
            _projectId = configurationHelper.ProjectId;
            _topicId = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
            _deadLetterTopicId = $"{_topicId}-deadletter";
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";
            _deadLetterSubscriptionId = $"{_subscriptionId}-deadletter";
            _serviceAccountName = configurationHelper.ServiceAccountName;

            _topicService.CreateTopic(_projectId, _topicId);

            _sut = new FluentQueueApi();
        }

        [TearDown]
        public void TearDown()
        {
            _sut.StopConsuming();

            _subscriptionService.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _topicId);
            _subscriptionService.DeleteSubscription(_projectId, _deadLetterSubscriptionId);
            _topicService.DeleteTopic(_projectId, _deadLetterTopicId);
        }

        [Test]
        public void Given_I_The_Message_Cannot_Be_Processed_When_I_Set_The_Retry_Policy_Then_The_Message_Is_Only_Handled_The_Correct_Amount_Of_Times()
        {
            //arrange
            const int maxAttempts = 5;
            const int minimumBackOffInSeconds = 10;
            const int maximumBackOffInSeconds = 10;

            var message1 = $"Hello, world {Guid.NewGuid()}";
            _messageHandler.SetResult(false);

            //act
            _sut
                .WithProject(_projectId)
                .WithTopic(_topicId)
                .WithRetryPolicy(_serviceAccountName, maxAttempts, minimumBackOffInSeconds, maximumBackOffInSeconds)
                .CreateSubscription(_subscriptionId, 60)
                .StartConsuming(_messageHandler, true);

            PublishMessage(message1);

            Thread.Sleep(120000); // allow time for processing the messages off the queue

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(0, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesNotHandled.Count == maxAttempts || _messageHandler.MessagesNotHandled.Count == maxAttempts + 1);
            Assert.IsTrue(_messageHandler.MessagesNotHandled.Contains(message1));
        }

        [Test]
        public void Given_I_The_Message_Cannot_Be_Processed_When_I_Set_The_Retry_Policy_Then_The_Message_Is_Taken_Off_The_Topic_And_Put_On_Deadletter_Topic()
        {
            //arrange
            const int maxAttempts = 5;
            const int minimumBackOffInSeconds = 10;
            const int maximumBackOffInSeconds = 10;

            var message1 = $"Hello, world {Guid.NewGuid()}";
            _messageHandler.SetResult(false);

            //act
            _sut
                .WithProject(_projectId)
                .WithTopic(_topicId)
                .WithRetryPolicy(_serviceAccountName, maxAttempts, minimumBackOffInSeconds, maximumBackOffInSeconds)
                .CreateSubscription(_subscriptionId, 60)
                .StartConsuming(_messageHandler, true);

            PublishMessage(message1);

            Thread.Sleep(120000); // allow time for processing the messages off the queue

            var message = _messageHelper.GetMessage(_projectId, _subscriptionId);
            var deadletterMessage = _messageHelper.GetMessage(_projectId, _deadLetterSubscriptionId);

            //assert
            Assert.IsNull(message);
            Assert.IsNotNull(deadletterMessage);
            Assert.AreEqual(deadletterMessage, message1);

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
