using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Api
{
    public class FluentApiCreateSubscriptionAndConsumeTests
    {
        private int _messageTimeoutInSeconds;

        private string _projectId;
        private string _topicId;
        private string _subscriptionId;

        private TestMessageHandler _messageHandler;
        private SubscriptionService _subscriptionService;
        private TopicService _topicService;

        private FluentQueueApi _sut;

        public FluentApiCreateSubscriptionAndConsumeTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _messageHandler = new TestMessageHandler();
            _topicService = new TopicService();
            _subscriptionService = new SubscriptionService();

            _projectId = "ons-blaise-dev";
            _topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _subscriptionId = $"blaise-nuget-subscription-{Guid.NewGuid()}";
            _messageTimeoutInSeconds = 60;

            _topicService.CreateTopic(_projectId, _topicId);

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

        private void PublishMessage(string message)
        {
            _sut
                .WithProject(_projectId)
                .WithTopic(_topicId)
                .Publish(message);
        }
    }
}
