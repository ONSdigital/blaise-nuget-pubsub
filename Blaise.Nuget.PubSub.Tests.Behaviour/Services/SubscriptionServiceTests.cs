using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class SubscriptionServiceTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscriptionId;
        private int _ackDeadlineInSeconds;

        private TopicService _topicService;

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
            _subscriptionId = string.Empty;
            _ackDeadlineInSeconds = 60;

            _topicService = new TopicService();

            _topicService.CreateTopic(_projectId, _topicId);

            _sut = new SubscriptionService();
        }

        [TearDown]
        public void TearDown()
        {
            if(!string.IsNullOrEmpty(_subscriptionId))
            {
                _sut.DeleteSubscription(_projectId, _subscriptionId);
            }

            _topicService.DeleteTopic(_projectId, _topicId);
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
            _subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackDeadlineInSeconds);

            //act
            var result = _sut.SubscriptionExists(_projectId, _subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void Given_A_Subscription_Doesnt_Exist_When_I_Call_CreateSubscription_The_Subscription_Is_Created()
        {
            //arrange
            _subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            Assert.IsFalse(_sut.SubscriptionExists(_projectId, _subscriptionId));

            //act
            _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackDeadlineInSeconds);

            //assert
            Assert.IsTrue(_sut.SubscriptionExists(_projectId, _subscriptionId));
        }

        [Test]
        public void Given_A_Subscription_Exists_When_I_Call_CreateSubscription_The_An_Exception_Is_Not_Thrown()
        {
            //arrange
            _subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackDeadlineInSeconds);
            Assert.IsTrue(_sut.SubscriptionExists(_projectId, _subscriptionId));

            //act && assert
            _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackDeadlineInSeconds);
        }

        [TestCase(10)]
        [TestCase(60)]
        [TestCase(600)]
        public void Given_ValidAckDeadline_When_I_Call_SubscriptionExists_Then_An_ArgumentOutOfRangeException_Is_Not_Thrown(int ackDeadlineInSeconds)
        {
            //arrange
            _subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            //act && assert
            Assert.DoesNotThrow(() => _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, ackDeadlineInSeconds));
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(601)]
        public void Given_InvalidAckDeadline_When_I_Call_SubscriptionExists_Then_An_ArgumentOutOfRangeException_Is_Thrown(int ackDeadlineInSeconds)
        {
            //arrange
            _subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, ackDeadlineInSeconds));
            Assert.AreEqual("The deadline for acking messages must be between '1' and '600'", exception.ParamName);
        }

        [Test]
        public void Given_A_Subscription_Exists_When_I_Call_DeleteSubscription_The_Subscription_Is_Deleted()
        {
            //arrange
            var subscriptionId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            _sut.CreateSubscription(_projectId, _topicId, subscriptionId, _ackDeadlineInSeconds);
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
    }
}
