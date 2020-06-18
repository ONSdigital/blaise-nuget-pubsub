using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Blaise.Nuget.PubSub.Tests.Unit.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Tests.Unit.Api
{
    public class FluentQueueApiTests
    {
        private Mock<IPublisherService> _publisherServiceMock;
        private Mock<ISubscriptionService> _subscriptionServiceMock;
        private Mock<ITopicService> _topicServiceMock;
        private Mock<ISubscriberService> _subscriberServiceMock;

        private IFluentQueueApi _sut;

        [SetUp]
        public void SetUpTests()
        {
            _publisherServiceMock = new Mock<IPublisherService>();
            _subscriptionServiceMock = new Mock<ISubscriptionService>();
            _topicServiceMock = new Mock<ITopicService>();
            _subscriberServiceMock = new Mock<ISubscriberService>();

            _sut = new FluentQueueApi(
                _publisherServiceMock.Object,
                _subscriptionServiceMock.Object,
                _topicServiceMock.Object,
                _subscriberServiceMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithProject_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var projectId = "Project123";

            //act
            var result = _sut.WithProject(projectId);

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
            var projectId = "Project123";
            var topicId = "Topic123";

            _topicServiceMock.Setup(p => p.CreateTopic(It.IsAny<string>(), It.IsAny<string>()));

            _sut.WithProject(projectId);

            //act
            var result = _sut.CreateTopic(topicId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentQueueApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateTopic_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";

            _topicServiceMock.Setup(p => p.CreateTopic(It.IsAny<string>(), It.IsAny<string>()));

            _sut.WithProject(projectId);

            //act
            _sut.CreateTopic(topicId);

            //assert
            _topicServiceMock.Verify(v => v.CreateTopic(projectId, topicId));
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
            //arrange
            var topicId = "Topic123";

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.CreateTopic(topicId));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithTopic_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var topicId = "Topic123";

            //act
            var result = _sut.WithTopic(topicId);

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
            var projectId = "Project123";
            var topicId = "Topic123";
            var message = "Message123";
            var attributes = new Dictionary<string, string>();

            _publisherServiceMock.Setup(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()));

            _sut.WithProject(projectId);
            _sut.WithTopic(topicId);

            //act
            _sut.Publish(message, attributes);

            //assert
            _publisherServiceMock.Verify(v => v.PublishMessage(projectId, topicId, message, attributes));
        }

        [Test]
        public void Given_Valid_Message_But_No_Attributes_When_I_Call_Publish_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var message = "Message123";

            _publisherServiceMock.Setup(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()));

            _sut.WithProject(projectId);
            _sut.WithTopic(topicId);

            //act
            _sut.Publish(message);

            //assert
            _publisherServiceMock.Verify(v => v.PublishMessage(projectId, topicId, message, null));
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
            var topicId = "Topic123";
            var message = "Message123";

            _sut.WithTopic(topicId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.Publish(message));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_TopicId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_Publish_Then_A_NullReferenceException_Is_Thrown()
        {
            //arrange
            var projectId = "Project123";
            var message = "Message123";

            _sut.WithProject(projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.Publish(message));
            Assert.AreEqual("The 'WithTopic' or 'CreateTopic' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateSubscription_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";
            var messageTimeoutInSeconds = 60;

            _subscriptionServiceMock.Setup(p => p.CreateSubscription(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 0));

            _sut.WithProject(projectId);
            _sut.WithTopic(topicId);

            //act
            _sut.CreateSubscription(subscriptionId, messageTimeoutInSeconds);

            //assert
            _subscriptionServiceMock.Verify(v => v.CreateSubscription(projectId, topicId, subscriptionId, messageTimeoutInSeconds));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateSubscription_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";
            var messageTimeoutInSeconds = 60;

            _sut.WithProject(projectId);
            _sut.WithTopic(topicId);

            //act
            var result = _sut.CreateSubscription(subscriptionId, messageTimeoutInSeconds);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentSubscriptionApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_An_Empty_SubscriptionId_When_I_Call_CreateSubscription_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange 
            var messageTimeoutInSeconds = 60;

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateSubscription(string.Empty, messageTimeoutInSeconds));
            Assert.AreEqual("A value for the argument 'subscriptionId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_SubscriptionId_When_I_Call_CreateSubscription_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange 
            var messageTimeoutInSeconds = 60;

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateSubscription(null, messageTimeoutInSeconds));
            Assert.AreEqual("subscriptionId", exception.ParamName);
        }

        [Test]
        public void Given_ProjectId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_CreateSubscription_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";
            var messageTimeoutInSeconds = 60;

            _sut.WithTopic(topicId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.CreateSubscription(subscriptionId, messageTimeoutInSeconds));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_TopicId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_CreateSubscription_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var projectId = "Project123";
            var subscriptionId = "Subscription123";
            var messageTimeoutInSeconds = 60;

            _sut.WithProject(projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.CreateSubscription(subscriptionId, messageTimeoutInSeconds));
            Assert.AreEqual("The 'WithTopic' or 'CreateTopic' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_WithSubscription_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";

            _sut.WithProject(projectId);
            _sut.WithTopic(topicId);

            //act
            var result = _sut.WithSubscription(subscriptionId);

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
        public void Given_A_Null_MessageHandler_When_I_Call_StartConsuming_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";

            _sut.WithProject(projectId);
            _sut.WithTopic(topicId);
            _sut.WithSubscription(subscriptionId);

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.StartConsuming( null));
            Assert.AreEqual("The argument 'messageHandler' must be supplied", exception.ParamName);
        }

        [Test]
        public void Given_Previous_Steps_Are_Setup_And_I_Do_Not_Supply_Throttle_When_I_Call_StartConsuming_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var subscriptionId = "Subscription123";
            var messageHandler = new TestMessageHandler();


            _subscriberServiceMock.Setup(s => s.StartConsuming(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMessageHandler>(), It.IsAny<bool>()));

            _sut.WithProject(projectId);
            _sut.WithSubscription(subscriptionId);

            //act
            _sut.StartConsuming(messageHandler);

            //assert
            _subscriberServiceMock.Verify(v => v.StartConsuming(projectId, subscriptionId, messageHandler, false));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_Previous_Steps_Are_Setup_And_I_Supply_Throttle_When_I_Call_StartConsuming_Then_It_Calls_The_Correct_Service_Method(bool throttle)
        {
            //arrange
            var projectId = "Project123";
            var subscriptionId = "Subscription123";
            var messageHandler = new TestMessageHandler();


            _subscriberServiceMock.Setup(s => s.StartConsuming(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMessageHandler>(), It.IsAny<bool>()));

            _sut.WithProject(projectId);
            _sut.WithSubscription(subscriptionId);

            //act
            _sut.StartConsuming(messageHandler, throttle);

            //assert
            _subscriberServiceMock.Verify(v => v.StartConsuming(projectId, subscriptionId, messageHandler, throttle));
        }

        [Test]
        public void Given_TopicId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_StartConsuming_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var subscriptionId = "Subscription123";
            var messageHandler = new TestMessageHandler();

            _sut.WithSubscription(subscriptionId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.StartConsuming(messageHandler));
            Assert.AreEqual("The 'WithProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_SubscriptionId_Has_Not_Been_Set_In_A_Previous_Step_When_I_Call_StartConsuming_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var projectId = "Project123";
            var messageHandler = new TestMessageHandler();

            _sut.WithProject(projectId);

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
    }
}
