﻿using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class TopicServiceTests
    {
        private string _projectId;
        private string _topicId;

        private TopicService _sut;

        public TopicServiceTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _projectId = "ons-blaise-dev";
            _topicId = string.Empty;

            _sut = new TopicService();            
        }

        [TearDown]
        public void TearDown()
        {
            if (!string.IsNullOrEmpty(_topicId))
            {
                _sut.DeleteTopic(_projectId, _topicId);
            }           
        }

        [Test]
        public void Given_A_Topic_Doesnt_Exist_When_I_Call_TopicExists_Then_False_Is_Returned()
        {
            //arrange
            var topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            //act
            var result = _sut.TopicExists(_projectId, topicId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.IsFalse(result);
        }

        [Test]
        public void Given_A_Topic_Exists_When_I_Call_TopicExists_Then_True_Is_Returned()
        {
            //arrange
            _topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            _sut.CreateTopic(_projectId, _topicId);

            //act
            var result = _sut.TopicExists(_projectId, _topicId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void Given_A_Topic_Doesnt_Exist_When_I_Call_CreateTopic_The_Topic_Is_Created()
        {
            //arrange
            _topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            Assert.IsFalse(_sut.TopicExists(_projectId, _topicId));

            //act
            _sut.CreateTopic(_projectId, _topicId);

            //assert
            Assert.IsTrue(_sut.TopicExists(_projectId, _topicId));
        }

        [Test]
        public void Given_A_Topic_Exists_When_I_Call_CreateTopic_The_An_Exception_Is_Not_Thrown()
        {
            //arrange
            _topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            _sut.CreateTopic(_projectId, _topicId);
            Assert.IsTrue(_sut.TopicExists(_projectId, _topicId));

            //act && assert
            _sut.CreateTopic(_projectId, _topicId);
        }

        [Test]
        public void Given_A_Topic_Exists_When_I_Call_DeleteTopic_The_Topic_Is_Deleted()
        {
            //arrange
            var topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";

            _sut.CreateTopic(_projectId, topicId);
            Assert.IsTrue(_sut.TopicExists(_projectId, topicId));

            //act
            _sut.DeleteTopic(_projectId, topicId);

            //assert
            Assert.IsFalse(_sut.TopicExists(_projectId, topicId));
        }

        [Test]
        public void Given_A_Topic_Doesnt_Exist_When_I_Call_DeleteTopic_The_An_Exception_Is_Not_Thrown()
        {
            //arrange
            var topicId = $"blaise-nuget-topic-{Guid.NewGuid()}";
            
            Assert.IsFalse(_sut.TopicExists(_projectId, topicId));

            //act && assert
            _sut.DeleteTopic(_projectId, topicId);
        }
    }
}
