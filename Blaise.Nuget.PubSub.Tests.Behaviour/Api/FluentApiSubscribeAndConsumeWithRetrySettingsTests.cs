using System;
using System.Threading;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Api
{
    public class FluentApiSubscribeAndConsumeWithRetrySettingsTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscriptionId;

        private TestMessageHandler _messageHandler;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;
        
        private FluentQueueApi _sut;

        public FluentApiSubscribeAndConsumeWithRetrySettingsTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _messageHandler = new TestMessageHandler();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService(new DeadLetterService(_topicService));

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
            _sut.StopConsuming();
            _subscriptionService.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _topicId);
            _topicService.DeleteTopic(_projectId, $"{_topicId}-deadletter");
        }

        [Test]
        public void Given_I_The_Message_Cannot_Be_Processed_When_I_Set_The_Retry_Policy_Then_It_Behaves_As_Expected()
        {
            //arrange
            var message1 = $"Hello, world {Guid.NewGuid()}";
            _messageHandler.SetResult(false);

            //act
            _sut
                .WithProject(_projectId)
                .WithTopic(_topicId)
                .WithRetryPolicy(5, 10,10)
                .CreateSubscription(_subscriptionId, 60)
                .StartConsuming(_messageHandler, true);

            PublishMessage(message1);

            Thread.Sleep(60000); // allow time for processing the messages off the queue

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(0, _messageHandler.MessagesHandled.Count);
            //Assert.AreEqual(5, _messageHandler.MessagesNotHandled.Count);
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
