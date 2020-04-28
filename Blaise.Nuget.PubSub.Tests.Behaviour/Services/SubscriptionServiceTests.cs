using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
using System.Threading;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class SubscriptionServiceTests
    {
        private string _projectId;
        private string _topicId;

        private TestMessageHandler _messageHandler;
        private PublishService _publishService;

        private SubscriptionService _sut;

        public SubscriptionServiceTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _projectId = "ons-blaise-dev";
            _topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            _messageHandler = new TestMessageHandler();
            _publishService = new PublishService();

            _publishService.CreateTopic(_projectId, _topicId);

            _sut = new SubscriptionService();
        }

        [TearDown]
        public void TearDown()
        {
            _publishService.DeleteTopic(_projectId, _topicId);
            _sut = new SubscriptionService();
        }

        [Test]
        public void Given_Three_Messages_Are_Available_When_I_Call_StartConsuming_Then_The_Three_Messages_Are_Processed()
        {
            //arrange
            var subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _sut.CreateSubscription(_projectId, _topicId, subscriptionId);

            var message1 = $"Hello, world {Guid.NewGuid()}";
            var message2 = $"Why, Hello {Guid.NewGuid()}";
            var message3 = $"Yo, Yo {Guid.NewGuid()}";

            PublishMessage(message1);
            PublishMessage(message2);
            PublishMessage(message3);

            //act
            _sut.StartConsuming(_projectId, subscriptionId, _messageHandler);

            Thread.Sleep(5000); // allow time for processing the messages off the queue

            _sut.StopConsuming();
            _sut.DeleteSubscription(_projectId, subscriptionId);

            //assert
            Assert.IsNotNull(_messageHandler.MessagesHandled);
            Assert.AreEqual(3, _messageHandler.MessagesHandled.Count);
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message1));
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message2));
            Assert.IsTrue(_messageHandler.MessagesHandled.Contains(message3));
        }

        [Test]
        public void Given_No_Subscriptions_When_I_Call_StopConsuming_Then_InvalidOperationException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<InvalidOperationException>(() => _sut.StopConsuming());
            Assert.AreEqual("No subscriptons have been setup", exception.Message);

        }

        [Test]
        public void Given_A_Subscription_Doesnt_Exist_When_I_Call_SubscriptionExists_Then_False_Is_Returned()
        {
            //arrange
            var subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            //act
            var result = _sut.SubscriptionExists(_projectId, subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.IsFalse(result);
        }

        [Test]
        public void Given_A_Subscription_Exists_When_I_Call_TopicExists_Then_True_Is_Returned()
        {
            //arrange
            var subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _sut.CreateSubscription(_projectId, _topicId, subscriptionId);

            //act
            var result = _sut.SubscriptionExists(_projectId, subscriptionId);
            _sut.DeleteSubscription(_projectId, subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void Given_A_Subscription_Doesnt_Exist_When_I_Call_CreateSubscription_The_Subscription_Is_Created()
        {
            //arrange
            var subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            Assert.IsFalse(_sut.SubscriptionExists(_projectId, subscriptionId));

            //act
            _sut.CreateSubscription(_projectId, _topicId, subscriptionId);

            //assert
            Assert.IsTrue(_sut.SubscriptionExists(_projectId, subscriptionId));

            _sut.DeleteSubscription(_projectId, subscriptionId);
        }

        [Test]
        public void Given_A_Subscription_Exists_When_I_Call_CreateSubscription_The_An_Exception_Is_Not_Thrown()
        {
            //arrange
            var subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            _sut.CreateSubscription(_projectId, _topicId, subscriptionId);
            Assert.IsTrue(_sut.SubscriptionExists(_projectId, subscriptionId));

            //act && assert
            _sut.CreateSubscription(_projectId, _topicId, subscriptionId);
            _sut.DeleteSubscription(_projectId, subscriptionId);
        }

        [Test]
        public void Given_A_Subscription_Exists_When_I_Call_DeleteSubscription_The_Subscription_Is_Deleted()
        {
            //arrange
            var subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            _sut.CreateSubscription(_projectId, _topicId, subscriptionId);
            Assert.IsTrue(_sut.SubscriptionExists(_projectId, subscriptionId));

            //act
            _sut.DeleteSubscription(_projectId, subscriptionId);

            //assert
            Assert.IsFalse(_sut.SubscriptionExists(_projectId, subscriptionId));
        }

        [Test]
        public void Given_A_Subscription_Doesnt_Exist_When_I_Call_DeleteSubscription_The_An_Exception_Is_Not_Thrown()
        {
            //arrange
            var subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            Assert.IsFalse(_sut.SubscriptionExists(_projectId, subscriptionId));

            //act && assert
            _sut.DeleteSubscription(_projectId, subscriptionId);
        }

        private void PublishMessage(string message)
        {
            _publishService.PublishMessage(_projectId, _topicId, message);
        }
    }
}
