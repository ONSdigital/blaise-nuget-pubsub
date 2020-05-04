using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class SubscriptionServiceDeadLetterTests
    {
        private string _projectId;
        private string _topicId;
        private string _deadLetterTopicId;
        private string _subscriptionId;
        private int _messageTimeoutInSeconds;

        private TopicService _topicService;

        private SubscriptionService _sut;

        public SubscriptionServiceDeadLetterTests()
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
            _messageTimeoutInSeconds = 60;

            _topicService = new TopicService();
            _topicService.CreateTopic(_projectId, _topicId);

            _sut = new SubscriptionService(_topicService);
        }

        [TearDown]
        public void TearDown()
        {
            _sut.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _deadLetterTopicId);
            _topicService.DeleteTopic(_projectId, _topicId);
        }

        [Test]
        public void Given_A_MaxNumberOfRetries_When_I_Call_CreateSubscription_The_Subscription_Is_Created_With_A_DeadLetter_Policy()
        {
            //arrange
            var maxNumberOfRetries = 5;

            //act
            _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, _messageTimeoutInSeconds, maxNumberOfRetries);

            //assert
            Assert.IsTrue(_sut.SubscriptionExists(_projectId, _subscriptionId));
            Assert.IsTrue(_topicService.TopicExists(_projectId, _deadLetterTopicId));
        }

        [TestCase(5)]
        [TestCase(10)]
        [TestCase(100)]
        public void Given_ValidMaxNumberOfRetries_When_I_Call_CreateSubscription_Then_An_ArgumentOutOfRangeException_Is_Not_Thrown(int maxNumberOfRetries)
        {
            //arrange
            _subscriptionId = $"blaise-nuget-subscription-{Guid.NewGuid()}";

            //act && assert
            Assert.DoesNotThrow(() => _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, 60, maxNumberOfRetries));
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(4)]
        [TestCase(101)]
        public void Given_InvalidMaxNumberOfRetries_When_I_Call_CreateSubscription_Then_An_ArgumentOutOfRangeException_Is_Thrown(int maxNumberOfRetries)
        {
            //arrange
            _subscriptionId = $"blaise-nuget-subscription-{Guid.NewGuid()}";

            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, 60, maxNumberOfRetries));
            Assert.AreEqual("The maximum number of retries for processing messages must be between '5' and '100'", exception.ParamName);
        }
    }
}
