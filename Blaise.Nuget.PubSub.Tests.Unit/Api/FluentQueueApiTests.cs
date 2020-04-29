using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Contracts.Enums;
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
        public void Given_Valid_Arguments_When_I_Call_ForProject_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var projectId = "Project123";

            //act
            var result = _sut.ForProject(projectId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentQueueApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_A_Null_ProjectId_When_I_Call_ForProject_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ForProject(string.Empty));
            Assert.AreEqual("A value for the argument 'projectId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_An_Empty_ProjectId_When_I_Call_ForProject_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.ForProject(null));
            Assert.AreEqual("projectId", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_ForTopic_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";

            _sut.ForProject(projectId);

            //act
            var result = _sut.ForTopic(topicId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentQueueApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_ForTopic_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";

            _topicServiceMock.Setup(p => p.CreateTopic(It.IsAny<string>(), It.IsAny<string>()));
            
            _sut.ForProject(projectId);

            //act
            _sut.ForTopic(topicId);

            //assert
            _topicServiceMock.Verify(v => v.CreateTopic(projectId, topicId));
        }

        [Test]
        public void Given_A_Null_TopicId_When_I_Call_ForTopic_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ForTopic(string.Empty));
            Assert.AreEqual("A value for the argument 'topicId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_An_Empty_TopicId_When_I_Call_ForTopic_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.ForTopic(null));
            Assert.AreEqual("topicId", exception.ParamName);
        }

        [Test]
        public void Given_ForProject_Has_Not_Been_Called_In_A_Previous_Step_When_I_Call_ForTopic_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var topicId = "Topic123";

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.ForTopic(topicId));
            Assert.AreEqual("The 'ForProject' step needs to be called prior to this", exception.Message);
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

            _sut.ForProject(projectId);
            _sut.ForTopic(topicId);

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

            _sut.ForProject(projectId);
            _sut.ForTopic(topicId);

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
        public void Given_ForTopic_Has_Not_Been_Called_In_A_Previous_Step_When_I_Call_Publish_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var projectId = "Project123";
            var message = "Message123";

            _sut.ForProject(projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.Publish(message));
            Assert.AreEqual("The 'ForTopic' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_ForSubscription_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";
            var ackDeadlineInSeconds = 60;

            _subscriptionServiceMock.Setup(p => p.CreateSubscription(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

            _sut.ForProject(projectId);
            _sut.ForTopic(topicId);

            //act
            _sut.ForSubscription(subscriptionId, ackDeadlineInSeconds);

            //assert
            _subscriptionServiceMock.Verify(v => v.CreateSubscription(projectId, topicId, subscriptionId, ackDeadlineInSeconds));
        }

        [Test]
        public void Given_No_AckDeadline_When_I_Call_ForSubscription_Then_It_Uses_The_Default_Value_Of_60()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";

            _subscriptionServiceMock.Setup(p => p.CreateSubscription(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

            _sut.ForProject(projectId);
            _sut.ForTopic(topicId);

            //act
            _sut.ForSubscription(subscriptionId);

            //assert
            _subscriptionServiceMock.Verify(v => v.CreateSubscription(projectId, topicId, subscriptionId, 60));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_ForSubscription_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";

            _sut.ForProject(projectId);
            _sut.ForTopic(topicId);

            //act
            var result = _sut.ForSubscription(subscriptionId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentSubscriptionApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_A_Null_SubscriptionId_When_I_Call_ForSubscription_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ForSubscription(string.Empty));
            Assert.AreEqual("A value for the argument 'subscriptionId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_An_Empty_SubscriptionId_When_I_Call_ForSubscription_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.ForSubscription(null));
            Assert.AreEqual("subscriptionId", exception.ParamName);
        }


        [Test]
        public void Given_ForTopic_Has_Not_Been_Called_In_A_Previous_Step_When_I_Call_ForSubscription_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var projectId = "Project123";
            var subscriptionId = "Subscription123";

            _sut.ForProject(projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.ForSubscription(subscriptionId));
            Assert.AreEqual("The 'ForTopic' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_A_Null_MessageHandler_When_I_Call_Consume_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";

            _sut.ForProject(projectId);
            _sut.ForTopic(topicId);
            _sut.ForSubscription(subscriptionId);

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.StartConsuming( null));
            Assert.AreEqual("The argument 'messageHandler' must be supplied", exception.ParamName);
        }

        [Test]
        public void Given_Previous_Steps_Are_Setup_When_I_Call_Consume_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var subscriptionId = "Subscription123";
            var messageHandler = new TestMessageHandler();


            _subscriberServiceMock.Setup(s => s.StartConsuming(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMessageHandler>()));

            _sut.ForProject(projectId);
            _sut.ForTopic(topicId);
            _sut.ForSubscription(subscriptionId);

            //act
            _sut.StartConsuming(messageHandler);

            //assert
            _subscriberServiceMock.Verify(v => v.StartConsuming(projectId, subscriptionId, messageHandler));
        }

        [Test]
        public void Given_ForSubscription_Has_Not_Called_In_A_Previous_Step_When_I_Call_Consume_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var projectId = "Project123";
            var messageHandler = new TestMessageHandler();

            _sut.ForProject(projectId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.StartConsuming(messageHandler));
            Assert.AreEqual("The 'ForSubscription' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_A_Subsription_When_I_Call_Consume_Then_It_Calls_The_Correct_Service_Method()
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
