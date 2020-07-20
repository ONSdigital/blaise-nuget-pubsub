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
        private int _ackTimeoutInSeconds;

        private TopicService _topicService;

        private SubscriptionService _sut;

        public SubscriptionServiceTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            var configurationHelper = new ConfigurationHelper();
            _projectId = configurationHelper.ProjectId;
            _topicId = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
           
            _subscriptionId = string.Empty;
            _ackTimeoutInSeconds = 60;

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
        public void Given_A_Subscription_Does_Not_Exist_When_I_Call_SubscriptionExists_Then_False_Is_Returned()
        {
            //arrange
            var configurationHelper = new ConfigurationHelper();
            var subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

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
            var configurationHelper = new ConfigurationHelper();
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";
            _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackTimeoutInSeconds);

            //act
            var result = _sut.SubscriptionExists(_projectId, _subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void Given_A_Subscription_Does_Not_Exist_When_I_Call_CreateSubscription_The_Subscription_Is_Created()
        {
            //arrange
            var configurationHelper = new ConfigurationHelper();
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";
            Assert.IsFalse(_sut.SubscriptionExists(_projectId, _subscriptionId));

            //act
            _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackTimeoutInSeconds);

            //assert
            Assert.IsTrue(_sut.SubscriptionExists(_projectId, _subscriptionId));
        }

        [Test]
        public void Given_A_Subscription_Exists_When_I_Call_CreateSubscription_Then_An_Exception_Is_Not_Thrown()
        {
            //arrange
            var configurationHelper = new ConfigurationHelper();
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackTimeoutInSeconds);
            Assert.IsTrue(_sut.SubscriptionExists(_projectId, _subscriptionId));

            //act && assert
            _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, _ackTimeoutInSeconds);
        }

        [TestCase(10)]
        [TestCase(60)]
        [TestCase(600)]
        public void Given_Valid_AckTimeoutInSeconds_When_I_Call_CreateSubscription_Then_An_ArgumentOutOfRangeException_Is_Not_Thrown(int ackTimeoutInSeconds)
        {
            //arrange
            var configurationHelper = new ConfigurationHelper();
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            //act && assert
            Assert.DoesNotThrow(() => _sut.CreateSubscription(_projectId, _topicId, _subscriptionId, ackTimeoutInSeconds));
        }

        [Test]
        public void Given_A_Subscription_Exists_When_I_Call_DeleteSubscription_The_Subscription_Is_Deleted()
        {
            //arrange
            var configurationHelper = new ConfigurationHelper();
            var subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            _sut.CreateSubscription(_projectId, _topicId, subscriptionId, _ackTimeoutInSeconds);
            Assert.IsTrue(_sut.SubscriptionExists(_projectId, subscriptionId));

            //act
            _sut.DeleteSubscription(_projectId, subscriptionId);

            //assert
            Assert.IsFalse(_sut.SubscriptionExists(_projectId, subscriptionId));
        }

        [Test]
        public void Given_A_Subscription_Does_Not_Exist_When_I_Call_DeleteSubscription_The_An_Exception_Is_Not_Thrown()
        {
            //arrange
            var configurationHelper = new ConfigurationHelper();
            var subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            Assert.IsFalse(_sut.SubscriptionExists(_projectId, subscriptionId));

            //act && assert
            _sut.DeleteSubscription(_projectId, subscriptionId);
        }
    }
}
