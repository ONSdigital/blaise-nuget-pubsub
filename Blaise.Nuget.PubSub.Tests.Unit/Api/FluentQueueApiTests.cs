using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Blaise.Nuget.PubSub.Tests.Unit.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Tests.Unit.Api
{
    public class FluentQueueApiTests
    {
        private Mock<IPublisherService> _publisherServiceMock;
        private Mock<ISubscriptionService> _subscriptionServiceMock;
        private Mock<ITopicService> _topicServiceMock;
        private Mock<ISubscriberService> _subscriberServiceMock;
        private Mock<IDeadLetterService> _deadLetterServiceMock;
        private Mock<IExponentialBackOffService> _backOffMock;

        private readonly string _projectId;
        private readonly string _topicId;
        private readonly string _subscriptionId;
        private readonly string _message;

        private IFluentQueueApi _sut;

        public FluentQueueApiTests()
        {
            _projectId = "Project123";
            _topicId = "Topic123";
            _subscriptionId = "Subscription123";
            _message = "Message123";
        }

        [SetUp]
        public void SetUpTests()
        {
            _publisherServiceMock = new Mock<IPublisherService>();
            _subscriptionServiceMock = new Mock<ISubscriptionService>();
            _topicServiceMock = new Mock<ITopicService>();
            _subscriberServiceMock = new Mock<ISubscriberService>();
            _deadLetterServiceMock = new Mock<IDeadLetterService>();
            _backOffMock = new Mock<IExponentialBackOffService>();

            _sut = new FluentQueueApi(
                _publisherServiceMock.Object,
                _subscriptionServiceMock.Object,
                _topicServiceMock.Object,
                _subscriberServiceMock.Object,
                _deadLetterServiceMock.Object,
                _backOffMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithProject_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //act
            var result = _sut.WithProject(_projectId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentQueueApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_An_Empty_ProjectId_When_I_Call_WithProject_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.WithProject(string.Empty));
            Assert.AreEqual("A value for the argument 'projectId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ProjectId_When_I_Call_WithProject_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.WithProject(null));
            Assert.AreEqual("projectId", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateTopic_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            _topicServiceMock.Setup(p => p.CreateTopic(It.IsAny<string>(), It.IsAny<string>()));

            _sut.WithProject(_projectId);

            //act
            var result = _sut.CreateTopic(_topicId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentQueueApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateTopic_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            _topicServiceMock.Setup(p => p.CreateTopic(It.IsAny<string>(), It.IsAny<string>()));

            _sut.WithProject(_projectId);

            //act
            _sut.CreateTopic(_topicId);

            //assert
            _topicServiceMock.Verify(v => v.CreateTopic(_projectId, _topicId));
        }

        [Test]
        public void Given_An_Empty_TopicId_When_I_Call_CreateTopic_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateTopic(string.Empty));
            Assert.AreEqual("A value for the argument 'topicId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_TopicId_When_I_Call_CreateTopic_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateTopic(null));
            Assert.AreEqual("topicId", exception.ParamName);
        }

        [Test]
        public void Given_WithProject_Has_Not_Been_Called_In_A_Previous_Step_When_I_Call_CreateTopic_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.CreateTopic(_topicId));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithTopic_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //act
            var result = _sut.WithTopic(_topicId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentQueueApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_An_Empty_TopicId_When_I_Call_WithTopic_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.WithTopic(string.Empty));
            Assert.AreEqual("A value for the argument 'topicId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_TopicId_When_I_Call_WithTopic_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.WithTopic(null));
            Assert.AreEqual("topicId", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_Publish_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var attributes = new Dictionary<string, string>();

            _publisherServiceMock.Setup(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<Dictionary<string, string>>()));

            _sut.WithProject(_projectId);
            _sut.WithTopic(_topicId);

            //act
            _sut.Publish(_message, attributes);

            //assert
            _publisherServiceMock.Verify(v => v.PublishMessage(_projectId, _topicId, _message, attributes));
        }

        [Test]
        public void Given_Valid_Message_But_No_Attributes_When_I_Call_Publish_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            _publisherServiceMock.Setup(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<Dictionary<string, string>>()));

            _sut.WithProject(_projectId);
            _sut.WithTopic(_topicId);

            //act
            _sut.Publish(_message);

            //assert
            _publisherServiceMock.Verify(v => v.PublishMessage(_projectId, _topicId, _message, null));
        }

        [Test]
        public void Given_A_Null_Message_When_I_Call_Publish_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.Publish(null));
            Assert.AreEqual("message", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_Message_When_I_Call_Publish_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.Publish(string.Empty));
            Assert.AreEqual("A value for the argument 'message' must be supplied", exception.Message);
        }

        [Test]
        public void Given_WithProject_Has_Not_Been_Called_In_A_Previous_Step_When_I_Call_Publish_Then_A_NullReferenceException_Is_Thrown()
        {
            //arrange
            _sut.WithTopic(_topicId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.Publish(_message));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_TopicId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_Publish_Then_A_NullReferenceException_Is_Thrown()
        {
            //arrange

            _sut.WithProject(_projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.Publish(_message));
            Assert.AreEqual("The 'WithTopic' or 'CreateTopic' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateSubscription_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var ackTimeoutInSeconds = 60;

            _subscriptionServiceMock.Setup(p => p.CreateSubscription(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<int>()));

            _sut.WithProject(_projectId);
            _sut.WithTopic(_topicId);

            //act
            _sut.CreateSubscription(_subscriptionId, ackTimeoutInSeconds);

            //assert
            _subscriptionServiceMock.Verify(v => v.CreateSubscription(_projectId, _topicId, _subscriptionId, 
                It.IsAny<int>()));
        }

        [TestCase(10)]
        [TestCase(20)]
        [TestCase(100)]
        public void Given_A_TimeOut_Is_Provided_When_I_Call_CreateSubscription_Then_The_Correct_Time_Is_Used(int ackTimeOutInSeconds)
        {
            //arrange
            _subscriptionServiceMock.Setup(p => p.CreateSubscription(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int>()));

            _sut.WithProject(_projectId);
            _sut.WithTopic(_topicId);

            //act
            _sut.CreateSubscription(_subscriptionId, ackTimeOutInSeconds);

            //assert
            _subscriptionServiceMock.Verify(v => v.CreateSubscription(_projectId, _topicId, _subscriptionId,
                ackTimeOutInSeconds));
        }

        [Test]
        public void Given_No_TimeOut_Is_Provided_When_I_Call_CreateSubscription_Then_The_Correct_Default_Time_Is_Used()
        {
            //arrange
            _subscriptionServiceMock.Setup(p => p.CreateSubscription(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<int>()));

            _sut.WithProject(_projectId);
            _sut.WithTopic(_topicId);

            //act
            _sut.CreateSubscription(_subscriptionId);

            //assert
            _subscriptionServiceMock.Verify(v => v.CreateSubscription(_projectId, _topicId, _subscriptionId,
                600));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateSubscription_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var ackTimeoutInSeconds = 60;

            _sut.WithProject(_projectId);
            _sut.WithTopic(_topicId);

            //act
            var result = _sut.CreateSubscription(_subscriptionId, ackTimeoutInSeconds);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentSubscriptionApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_An_Empty_SubscriptionId_When_I_Call_CreateSubscription_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange 
            var ackTimeoutInSeconds = 60;

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateSubscription(string.Empty, ackTimeoutInSeconds));
            Assert.AreEqual("A value for the argument 'subscriptionId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_SubscriptionId_When_I_Call_CreateSubscription_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange 
            var ackTimeoutInSeconds = 60;

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateSubscription(null, ackTimeoutInSeconds));
            Assert.AreEqual("subscriptionId", exception.ParamName);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(601)]
        public void Given_An_Invalid_AckTimeoutInSeconds_Value_When_I_Call_CreateSubscription_Then_An_ArgumentOutOfRangeException_Is_Thrown(int ackTimeoutInSeconds)
        {
            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.CreateSubscription(_subscriptionId, ackTimeoutInSeconds));
            Assert.AreEqual($"The deadline for acking messages must be between the values '1' and '600'", exception.ParamName);
        }

        [Test]
        public void Given_ProjectId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_CreateSubscription_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var ackTimeoutInSeconds = 60;

            _sut.WithTopic(_topicId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.CreateSubscription(_subscriptionId, ackTimeoutInSeconds));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_TopicId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_CreateSubscription_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var ackTimeoutInSeconds = 60;

            _sut.WithProject(_projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.CreateSubscription(_subscriptionId, ackTimeoutInSeconds));
            Assert.AreEqual("The 'WithTopic' or 'CreateTopic' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithSubscription_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            _sut.WithProject(_projectId);
            _sut.WithTopic(_topicId);

            //act
            var result = _sut.WithSubscription(_subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentSubscriptionApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_An_Empty_SubscriptionId_When_I_Call_WithSubscription_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.WithSubscription(string.Empty));
            Assert.AreEqual("A value for the argument 'subscriptionId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_SubscriptionId_When_I_Call_WithSubscription_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.WithSubscription(null));
            Assert.AreEqual("subscriptionId", exception.ParamName);
        }

        [Test]
        public void Given_A_Null_MessageHandler_When_I_Call_StartConsuming_With_IMessageHandler_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            _sut.WithProject(_projectId);
            _sut.WithTopic(_topicId);
            _sut.WithSubscription(_subscriptionId);

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.StartConsuming((IMessageHandler) null));
            Assert.AreEqual("The argument 'messageHandler' must be supplied", exception.ParamName);
        }

        [Test]
        public void Given_Previous_Steps_Are_Setup_And_I_Do_Not_Supply_Throttle_When_I_Call_StartConsuming_With_IMessageHandler_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var messageHandler = new TestMessageHandler();


            _subscriberServiceMock.Setup(s => s.StartConsuming(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<IMessageHandler>(), It.IsAny<bool>()));

            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            //act
            _sut.StartConsuming(messageHandler);

            //assert
            _subscriberServiceMock.Verify(v => v.StartConsuming(_projectId, _subscriptionId, messageHandler, false));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_Previous_Steps_Are_Setup_And_I_Supply_Throttle_When_I_Call_StartConsuming_With_IMessageHandler_Then_It_Calls_The_Correct_Service_Method(bool throttle)
        {
            //arrange
            var messageHandler = new TestMessageHandler();


            _subscriberServiceMock.Setup(s => s.StartConsuming(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<IMessageHandler>(), It.IsAny<bool>()));

            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            //act
            _sut.StartConsuming(messageHandler, throttle);

            //assert
            _subscriberServiceMock.Verify(v => v.StartConsuming(_projectId, _subscriptionId, messageHandler, throttle));
        }

        [Test]
        public void Given_TopicId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_StartConsuming_With_IMessageHandler_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var messageHandler = new TestMessageHandler();

            _sut.WithSubscription(_subscriptionId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.StartConsuming(messageHandler));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_SubscriptionId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_StartConsuming_With_IMessageHandler_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var messageHandler = new TestMessageHandler();

            _sut.WithProject(_projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.StartConsuming(messageHandler));
            Assert.AreEqual("The 'WithSubscription' or 'CreateSubscription' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_A_Null_MessageHandler_When_I_Call_StartConsuming_With_IMessageTriggerHandler_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            _sut.WithProject(_projectId);
            _sut.WithTopic(_topicId);
            _sut.WithSubscription(_subscriptionId);

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.StartConsuming((IMessageTriggerHandler)null));
            Assert.AreEqual("The argument 'messageHandler' must be supplied", exception.ParamName);
        }

        [Test]
        public void Given_Previous_Steps_Are_Setup_When_I_Call_StartConsuming_With_MessageTriggerHandler_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var messageHandler = new TestMessageTriggerHandler();


            _subscriberServiceMock.Setup(s => s.StartConsuming(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IMessageTriggerHandler>()));

            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            //act
            _sut.StartConsuming(messageHandler);

            //assert
            _subscriberServiceMock.Verify(v => v.StartConsuming(_projectId, _subscriptionId, messageHandler));
        }

        [Test]
        public void Given_TopicId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_StartConsuming_With_MessageTriggerHandler_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var messageHandler = new TestMessageTriggerHandler();

            _sut.WithSubscription(_subscriptionId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.StartConsuming(messageHandler));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_SubscriptionId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_StartConsuming_With_MessageTriggerHandler_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var messageHandler = new TestMessageTriggerHandler();

            _sut.WithProject(_projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.StartConsuming(messageHandler));
            Assert.AreEqual("The 'WithSubscription' or 'CreateSubscription' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_A_Subscription_When_I_Call_StopConsuming_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            _subscriberServiceMock.Setup(s => s.StopConsuming());

            //act
            _sut.StopConsuming();

            //assert
            _subscriberServiceMock.Verify(v => v.StopConsuming());
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithExponentialBackOff_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var minimumBackOffInSeconds = 20;
            var maximumBackOffInSeconds = 30;

            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            //act
            _sut.WithExponentialBackOff(minimumBackOffInSeconds, maximumBackOffInSeconds);

            //assert
            _backOffMock.Verify(v => v.UpdateSubscriptionWithExponentialBackOff(_projectId, _subscriptionId,
                minimumBackOffInSeconds, maximumBackOffInSeconds), Times.Once);
        }

        [TestCase(10, 20)]
        [TestCase(30, 300)]
        [TestCase(10, 600)]
        public void Given_Min_And_Max_Values_Are_Provided_When_I_Call_WithExponentialBackOff_Then_The_Correct_Time_Is_Used(int minimumBackOffInSeconds, int maximumBackOffInSeconds)
        {
            //arrange
            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            //act
            _sut.WithExponentialBackOff(minimumBackOffInSeconds, maximumBackOffInSeconds);

            //assert
            _backOffMock.Verify(v => v.UpdateSubscriptionWithExponentialBackOff(_projectId, _subscriptionId,
                minimumBackOffInSeconds, maximumBackOffInSeconds), Times.Once);
        }

        [Test]
        public void Given_No_Min_And_Max_Values_Are_Provided_When_I_Call_WithExponentialBackOff_Then_The_Correct_Default_Time_Is_Used()
        {
            //arrange
            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            //act
            _sut.WithExponentialBackOff();

            //assert
            _backOffMock.Verify(v => v.UpdateSubscriptionWithExponentialBackOff(_projectId, _subscriptionId,
                10, 600), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithExponentialBackOff_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            //act
            var result = _sut.WithExponentialBackOff();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentSubscriptionApi>(result);
            Assert.AreSame(_sut, result);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(601)]
        public void Given_Invalid_MinimumBackOffInSeconds_Value_When_I_Call_WithExponentialBackOff_Then_An_ArgumentOutOfRangeException_Is_Thrown(int minimumBackOffInSeconds)
        {
            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.WithExponentialBackOff(minimumBackOffInSeconds));
            Assert.AreEqual("minimumBackOffInSeconds", exception.ParamName);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(601)]
        public void Given_Invalid_MaximumBackOffInSeconds_Value_When_I_Call_WithExponentialBackOff_Then_An_ArgumentOutOfRangeException_Is_Thrown(int maximumBackOffInSeconds)
        {
            //arrange
            var minimumBackOffInSeconds = 10;

            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.WithExponentialBackOff(minimumBackOffInSeconds, maximumBackOffInSeconds));
            Assert.AreEqual("maximumBackOffInSeconds", exception.ParamName);
        }


        [Test]
        public void Given_ProjectId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_WithExponentialBackOff_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            _sut.WithSubscription(_subscriptionId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.WithExponentialBackOff());
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_SubscriptionId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_CreateSubscription_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            _sut.WithProject(_projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.WithExponentialBackOff());
            Assert.AreEqual("The 'WithSubscription' or 'CreateSubscription' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithDeadLetter_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var maximumDeliveryAttempts = 5;
            var deadLetterTopicId = "Topic1";
            var subscriptionName = "subscriptionName1";
            var subscription = new Subscription {Name = subscriptionName};

            _deadLetterServiceMock.Setup(d => d.UpdateSubscriptionWithDeadLetter(_projectId, _subscriptionId, deadLetterTopicId, maximumDeliveryAttempts))
                .Returns(subscription);

            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            //act
            _sut.WithDeadLetter(deadLetterTopicId, maximumDeliveryAttempts);

            //assert
            _deadLetterServiceMock.Verify(v => v.UpdateSubscriptionWithDeadLetter(_projectId, _subscriptionId,
                deadLetterTopicId, maximumDeliveryAttempts), Times.Once);
        }

        [TestCase(5)]
        [TestCase(30)]
        [TestCase(100)]
        public void Given_MaximumDeliveryAttempts_Value_Is_Provided_When_I_Call_WithDeadLetter_Then_The_Correct_Value_Is_Used(int maximumDeliveryAttempts)
        {
            //arrange
            var deadLetterTopicId = "Topic1";

            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            _deadLetterServiceMock.Setup(d => d.UpdateSubscriptionWithDeadLetter(_projectId, _subscriptionId, deadLetterTopicId, maximumDeliveryAttempts))
                .Returns(new Subscription());

            //act
            _sut.WithDeadLetter(deadLetterTopicId, maximumDeliveryAttempts);

            //assert
            _deadLetterServiceMock.Verify(v => v.UpdateSubscriptionWithDeadLetter(_projectId, _subscriptionId,
                deadLetterTopicId, maximumDeliveryAttempts), Times.Once);
        }

        [Test]
        public void Given_No_MaximumDeliveryAttempts_Value_Is_Provided_When_I_Call_WithDeadLetter_Then_The_Correct_Default_Value_Is_Used()
        {
            //arrange
            var deadLetterTopicId = "Topic1";

            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            _deadLetterServiceMock.Setup(d => d.UpdateSubscriptionWithDeadLetter(_projectId, _subscriptionId, deadLetterTopicId, It.IsAny<int>()))
                .Returns(new Subscription());

            //act
            _sut.WithDeadLetter(deadLetterTopicId);

            //assert
            _deadLetterServiceMock.Verify(v => v.UpdateSubscriptionWithDeadLetter(_projectId, _subscriptionId,
                deadLetterTopicId, 5), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithDeadLetter_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var deadLetterTopicId = "Topic1";

            _sut.WithProject(_projectId);
            _sut.WithSubscription(_subscriptionId);

            _deadLetterServiceMock.Setup(d => d.UpdateSubscriptionWithDeadLetter(_projectId, _subscriptionId, deadLetterTopicId, It.IsAny<int>()))
                .Returns(new Subscription());

            //act
            var result = _sut.WithDeadLetter(deadLetterTopicId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentSubscriptionApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_An_Empty_DeadLetterTopicId_When_I_Call_WithDeadLetter_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.WithDeadLetter(string.Empty));
            Assert.AreEqual("A value for the argument 'deadLetterTopicId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_deadLetterTopicId_When_I_Call_WithDeadLetter_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.WithDeadLetter(null));
            Assert.AreEqual("deadLetterTopicId", exception.ParamName);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(4)]
        [TestCase(101)]
        public void Given_An_Invalid_MaximumDeliveryAttempts_Value_When_I_Call_WithExponentialBackOff_Then_An_ArgumentOutOfRangeException_Is_Thrown(int maximumDeliveryAttempts)
        {
            //arrange
            var deadLetterTopicId = "Topic1";

            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.WithDeadLetter(deadLetterTopicId, maximumDeliveryAttempts));
            Assert.AreEqual($"maximumDeliveryAttempts", exception.ParamName);
        }
        

        [Test]
        public void Given_ProjectId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_WithDeadLetter_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var deadLetterTopicId = "Topic1";
            _sut.WithSubscription(_subscriptionId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.WithDeadLetter(deadLetterTopicId));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_SubscriptionId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_WithDeadLetter_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var deadLetterTopicId = "Topic1";
            _sut.WithProject(_projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.WithDeadLetter(deadLetterTopicId));
            Assert.AreEqual("The 'WithSubscription' or 'CreateSubscription' step needs to be called prior to this", exception.Message);
        }
    }
}
