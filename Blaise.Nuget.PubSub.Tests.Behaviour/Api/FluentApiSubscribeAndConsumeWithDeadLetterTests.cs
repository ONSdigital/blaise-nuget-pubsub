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
        private string _subscriptionId;

        private TestMessageHandler _messageHandler;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;
        
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

            var configurationHelper = new ConfigurationHelper();
            _projectId = configurationHelper.ProjectId;
            _topicId = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            _topicService.CreateTopic(_projectId, _topicId);

            _sut = new FluentQueueApi();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _sut.StopConsuming();
            }
            catch
            {
            }
            
            _subscriptionService.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _topicId);
            _subscriptionService.DeleteSubscription(_projectId, $"{_subscriptionId}-deadletter");
            _topicService.DeleteTopic(_projectId, $"{_topicId}-deadletter");
        }

        [Test]
        public void Given_I_The_Message_Cannot_Be_Processed_When_I_Set_The_Retry_Policy_Then_It_Behaves_As_Expected()
        {
            //arrange
            var maxAttempts = 5;
            var minimumBackOff = 10;
            var maximumBackOff = 10;

            var message1 = $"Hello, world {Guid.NewGuid()}";
            _messageHandler.SetResult(false);

            //act
            _sut
                .WithProject(_projectId)
                .WithTopic(_topicId)
                .WithRetryPolicy(maxAttempts, minimumBackOff, maximumBackOff)
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

        private void PublishMessage(string message)
        {
            _sut
                .WithProject(_projectId)
                .WithTopic(_topicId)
                .Publish(message);
        }
    }
}
