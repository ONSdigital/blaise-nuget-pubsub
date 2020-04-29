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

            _topicService = new TopicService();

            _topicService.CreateTopic(_projectId, _topicId);

            _sut = new SubscriptionService();
        }

        [TearDown]
        public void TearDown()
        {
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
    }
}
