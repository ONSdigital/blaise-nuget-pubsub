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
        private Mock<IPublishService> _publishServiceMock;
        private Mock<ISubscriptionService> _subscriptionServiceMock;
        private Mock<ISchedulerService> _schedulerServiceMock;

        private FluentQueueApi _sut;

        [SetUp]
        public void SetUpTests()
        {
            _publishServiceMock = new Mock<IPublishService>();
            _subscriptionServiceMock = new Mock<ISubscriptionService>();
            _schedulerServiceMock = new Mock<ISchedulerService>();

            _sut = new FluentQueueApi(
                _publishServiceMock.Object, 
                _subscriptionServiceMock.Object,
                _schedulerServiceMock.Object);
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
            var topicId = "Topic123";

            //act
            var result = _sut.ForTopic(topicId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentPublishApi>(result);
            Assert.AreSame(_sut, result);
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
        public void Given_Valid_Arguments_When_I_Call_Publish_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var message = "Message123";
            var attributes = new Dictionary<string, string>();

            _publishServiceMock.Setup(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()));

            _sut.ForProject(projectId);
            _sut.ForTopic(topicId);

            //act
            _sut.Publish(message, attributes);

            //assert
            _publishServiceMock.Verify(v => v.PublishMessage(projectId, topicId, message, attributes));
        }

        [Test]
        public void Given_Valid_Message_But_No_Attributes_When_I_Call_Publish_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var topicId = "Topic123";
            var message = "Message123";

            _publishServiceMock.Setup(p => p.PublishMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()));

            _sut.ForProject(projectId);
            _sut.ForTopic(topicId);

            //act
            _sut.Publish(message);

            //assert
            _publishServiceMock.Verify(v => v.PublishMessage(projectId, topicId, message, null));
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
        public void Given_ForProject_Has_Not_Been_Called_In_A_Previous_Step_When_I_Call_Publish_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var topicId = "Topic123";
            var message = "Message123";

            _sut.ForTopic(topicId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.Publish(message));
            Assert.AreEqual("The 'ForProject' step needs to be called prior to this", exception.Message);
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
        public void Given_Valid_Arguments_When_I_Call_ForSubscription_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            var subscriptionId = "Subscription123";

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
        public void Given_Valid_Arguments_When_I_Call_Consume_Then_It_Returns_Same_Instance_Of_Itself_Back()
        {
            //arrange
            int numberOfMessages = 1;
            var messageHandler = new TestMessageHandler();

            //act
            var result = _sut.Consume(numberOfMessages, messageHandler);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IFluentSubscriptionApi>(result);
            Assert.AreSame(_sut, result);
        }

        [Test]
        public void Given_A_Null_MessageHandler_When_I_Call_Consume_Then_An_ArgumentNullException_Is_Thrown()
        {   
            //arrange
            int numberOfMessages = 1;

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.Consume(numberOfMessages, null));
            Assert.AreEqual("The argument 'messageHandler' must be supplied", exception.ParamName);
        }



        [Test]
        public void Given_Previous_Steps_Are_Setup_When_I_Call_Now_Then_It_Calls_The_Correct_Service_Method()
        {
            //arrange
            var projectId = "Project123";
            var subscriptionId = "Subscription123";
            int numberOfMessages = 1;
            var messageHandler = new TestMessageHandler();


            _subscriptionServiceMock.Setup(s => s.Consume(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IMessageHandler>()));

            _sut.ForProject(projectId);
            _sut.ForSubscription(subscriptionId);
            _sut.Consume(numberOfMessages, messageHandler);

            //act
            _sut.Now();

            //assert
            _subscriptionServiceMock.Verify(v => v.Consume(projectId, subscriptionId, numberOfMessages, messageHandler));
        }

        [Test]
        public void Given_ForProject_Has_Not_Called_In_A_Previous_Step_When_I_Call_Now_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var subscriptionId = "Subscription123";
            int numberOfMessages = 1;
            var messageHandler = new TestMessageHandler();

            _sut.ForSubscription(subscriptionId);
            _sut.Consume(numberOfMessages, messageHandler);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.Now());
            Assert.AreEqual("The 'ForProject' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_ForSubscription_Has_Not_Called_In_A_Previous_Step_When_I_Call_Now_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var projectId = "Project123";
            int numberOfMessages = 1;
            var messageHandler = new TestMessageHandler();

            _sut.ForProject(projectId);
            _sut.Consume(numberOfMessages, messageHandler);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.Now());
            Assert.AreEqual("The 'ForSubscription' step needs to be called prior to this", exception.Message);
        }

        [Test]
        public void Given_Consume_Has_Not_Called_In_A_Previous_Step_When_I_Call_Now_Then_A_NullReferenceExceptionIs_Thrown()
        {
            //arrange
            var projectId = "Project123";
            var subscriptionId = "Subscription123";

            _sut.ForProject(projectId);
            _sut.ForSubscription(subscriptionId);

            //act && assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.Now());
            Assert.AreEqual("The 'Consume' step needs to be called prior to this", exception.Message);
        }
    }
}
