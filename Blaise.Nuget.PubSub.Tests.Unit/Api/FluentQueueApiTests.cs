using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Moq;
using NUnit.Framework;
using System;

namespace Blaise.Nuget.PubSub.Tests.Unit.Api
{
    public class FluentQueueApiTests
    {
        private Mock<IPublisherService> _publishServiceMock;
        private Mock<ISubscriptionService> _subscriptionServiceMock;

        private FluentQueueApi _sut;

        [SetUp]
        public void SetUpTests()
        {
            _publishServiceMock = new Mock<IPublisherService>();
            _subscriptionServiceMock = new Mock<ISubscriptionService>();

            _sut = new FluentQueueApi(_publishServiceMock.Object, _subscriptionServiceMock.Object);
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
        public void Given_A_Null_ProjectId_When_I_Call_ForProject_Then_An_ArgumentNullException_Is_Thrown()
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
        public void Given_A_Null_TopicId_When_I_Call_ForTopic_Then_An_ArgumentNullException_Is_Thrown()
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
    }
}
