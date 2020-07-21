using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
using Blaise.Nuget.PubSub.Core.Models;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class DeadLetterSubscriptionServiceTests
    {
        private string _projectId;
        private string _topicId;
        private string _subscriptionId;
        private int _ackTimeoutInSeconds;
        private string _serviceAccountName;
        private int _maxAttempts;
        private int _minimumBackOffInSeconds;
        private int _maximumBackOffInSeconds;

        private TopicService _topicService;
        private SubscriptionService _subscriptionService;

        private DeadLetterSubscriptionService _sut;

        public DeadLetterSubscriptionServiceTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            var configurationHelper = new ConfigurationHelper();
            _projectId = configurationHelper.ProjectId;
            _topicId = $"{configurationHelper.TopicId}-{Guid.NewGuid()}";
            _subscriptionId = $"{configurationHelper.SubscriptionId}-{Guid.NewGuid()}";

            _ackTimeoutInSeconds = 60;
            _serviceAccountName = configurationHelper.ServiceAccountName;
            _maxAttempts = 5;
            _minimumBackOffInSeconds = 10;
            _maximumBackOffInSeconds = 10;
            
            _topicService = new TopicService();
            _topicService.CreateTopic(_projectId, _topicId);

            _subscriptionService = new SubscriptionService();

            _sut = new DeadLetterSubscriptionService(_topicService, _subscriptionService);
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionService.DeleteSubscription(_projectId, _subscriptionId);
            _topicService.DeleteTopic(_projectId, _topicId);
            _subscriptionService.DeleteSubscription(_projectId, $"{_subscriptionId}-deadletter");
            _topicService.DeleteTopic(_projectId, $"{_topicId}-deadletter");
        }

        [Test]
        public void Given_Valid_Values_I_Call_CreateSubscriptionWithDeadLetter_The_Subscription_With_Deadletter_Is_Created()
        {
            //arrange
            var retrySettingsModel = new RetrySettingsModel(_serviceAccountName, _maxAttempts, _minimumBackOffInSeconds, _maximumBackOffInSeconds);

            //act
            _sut.CreateSubscriptionWithDeadLetter(_projectId, _topicId, _subscriptionId, _ackTimeoutInSeconds, retrySettingsModel);

            //assert
            Assert.IsTrue(_subscriptionService.SubscriptionExists(_projectId, _subscriptionId));
            Assert.IsTrue(_topicService.TopicExists(_projectId, $"{_topicId}-deadletter"));
            Assert.IsTrue(_subscriptionService.SubscriptionExists(_projectId, $"{_subscriptionId}-deadletter"));
        }
    }
}
