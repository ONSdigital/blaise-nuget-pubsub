using System;
using System.Threading;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Api
{
    public class FluentApiSingleDeadLetterTests
    {
        private string _projectId;
        private string _topic1Id;
        private string _topic2Id;
        private string _subscription1Id;
        private string _subscription2Id;

        private string _deadLetterTopicId;
        private string _deadLetterSubscriptionId;

        private TestMessageHandler _messageHandler1;
        private TestMessageHandler _messageHandler2;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;
        private MessageHelper _messageHelper;

        private FluentQueueApi _sut1;
        private FluentQueueApi _sut2;

        public FluentApiSingleDeadLetterTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _messageHandler1 = new TestMessageHandler();
            _messageHandler2 = new TestMessageHandler();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService();
            _messageHelper = new MessageHelper();

            var configurationHelper = new ConfigurationHelper();
            _projectId = configurationHelper.ProjectId;

            _topic1Id = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
            _subscription1Id = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";
            _topic2Id = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
            _subscription2Id = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            _deadLetterTopicId = $"{configurationHelper.DeadletterTopicId}-{Guid.NewGuid()}";
            _deadLetterSubscriptionId = $"{configurationHelper.DeadletterSubscriptionId}-{Guid.NewGuid()}";

            _topicService.CreateTopic(_projectId, _topic1Id);
            _topicService.CreateTopic(_projectId, _topic2Id);
            CreateDeadletterTopicAndSubscription();

            _sut1 = new FluentQueueApi();
            _sut2 = new FluentQueueApi();
        }
        
        private void CreateDeadletterTopicAndSubscription()
        {
            _topicService.CreateTopic(_projectId, _deadLetterTopicId);
            _subscriptionService.CreateSubscription(_projectId, _deadLetterTopicId, _deadLetterSubscriptionId, 600);
        }

        [TearDown]
        public void TearDown()
        {
            _sut1.StopConsuming();
            _sut2.StopConsuming();

            _subscriptionService.DeleteSubscription(_projectId, _subscription1Id);
            _topicService.DeleteTopic(_projectId, _topic1Id);

            _subscriptionService.DeleteSubscription(_projectId, _subscription2Id);
            _topicService.DeleteTopic(_projectId, _topic2Id);

            _subscriptionService.DeleteSubscription(_projectId, _deadLetterSubscriptionId);
            _topicService.DeleteTopic(_projectId, _deadLetterTopicId);
        }
        
        [Test]
        public void Given_The_Same_DeadLetter_For_Two_Subscriptions_When_A_Message_Fails_Then_The_Message__Put_On_The_One_Deadletter_Topic()
        {
            //arrange
            const int maxAttempts = 5;

            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Hello, world {Guid.NewGuid()}";
            _messageHandler1.SetResult(false);
            _messageHandler2.SetResult(false);

            //act
            _sut1
                .WithProject(_projectId)
                .WithTopic(_topic1Id)
                .CreateSubscription(_subscription1Id, 60)
                .WithDeadLetter(_deadLetterTopicId, maxAttempts)
                .StartConsuming(_messageHandler1, true);

            _sut2
                .WithProject(_projectId)
                .WithTopic(_topic2Id)
                .CreateSubscription(_subscription2Id, 60)
                .WithDeadLetter(_deadLetterTopicId, maxAttempts)
                .StartConsuming(_messageHandler2, true);

            PublishMessage(message1, _topic1Id);
            PublishMessage(message2, _topic2Id);

            Thread.Sleep(20000); // allow time for processing the messages off the queue

            var subscription1Message = _messageHelper.GetMessage(_projectId, _subscription1Id);
            var subscription2Message = _messageHelper.GetMessage(_projectId, _subscription2Id);

            Thread.Sleep(20000); // allow time for processing the messages onto the deadletter queue

            var deadletterMessage1 = _messageHelper.GetMessage(_projectId, _deadLetterSubscriptionId);
            var deadletterMessage2 = _messageHelper.GetMessage(_projectId, _deadLetterSubscriptionId);

            //assert
            Assert.IsNull(subscription1Message);
            Assert.IsNull(subscription2Message);
            Assert.IsNotNull(deadletterMessage1);
            Assert.IsNotNull(deadletterMessage2);
            Assert.IsTrue(deadletterMessage1 == message1 || deadletterMessage2 == message1);
            Assert.IsTrue(deadletterMessage1 == message2 || deadletterMessage2 == message2);
        }

        private void PublishMessage(string message, string topicId)
        {
            _sut1
                .WithProject(_projectId)
                .WithTopic(topicId)
                .Publish(message);
        }
    }
}
