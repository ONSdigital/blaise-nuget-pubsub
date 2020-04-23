using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
using System;
using System.Threading;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class SchedulerServiceTests
    {
        private TestSchedulerAction _testAction;
        private SchedulerService _sut;

        [SetUp]
        public void Setup()
        {
            _testAction = new TestSchedulerAction();
            _sut = new SchedulerService();
        }

        [Test]
        public void Given_Give_I_Schedule_An_Action_Then_The_Action_Is_Executed_Per_The_Schedule()
        {
            //arrange
            var intervalNumber = 10;
            var intervalType = IntervalType.Seconds;
            var scheduledDateTime = DateTime.Now;

            //act
            _sut.Schedule(() => _testAction.LogAction(scheduledDateTime), intervalNumber, intervalType);

            Thread.Sleep(30000);

            //assert
            Assert.IsNotNull(_testAction.ActionsIntervalLogged);
            Assert.AreEqual(4, _testAction.ActionsIntervalLogged.Count);
        }
    }
}
