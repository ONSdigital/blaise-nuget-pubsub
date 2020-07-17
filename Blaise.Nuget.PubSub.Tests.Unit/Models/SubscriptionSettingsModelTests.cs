using System;
using Blaise.Nuget.PubSub.Core.Models;
using NUnit.Framework;

namespace Blaise.Nuget.PubSub.Tests.Unit.Models
{
    public class SubscriptionSettingsModelTests
    {
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(599)]
        [TestCase(600)]
        public void Given_Valid_AckTimeoutInSeconds_When_I_Set_The_Value_Then_An_Exception_Is_Not_Thrown(int ackTimeoutInSeconds)
        {
            //arrange
            var subscriptionSettingsModel = new SubscriptionSettingsModel();

            //act && assert
            Assert.DoesNotThrow(() => subscriptionSettingsModel.AckTimeoutInSeconds = ackTimeoutInSeconds);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(601)]
        public void Given_Invalid_AckTimeoutInSeconds_When_I_Set_The_Value_Then_An_ArgumentOutOfRangeException_Is_Thrown(int ackTimeoutInSeconds)
        {
            //arrange
            var subscriptionSettingsModel = new SubscriptionSettingsModel();

            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => subscriptionSettingsModel.AckTimeoutInSeconds = ackTimeoutInSeconds);
            Assert.AreEqual("AckTimeoutInSeconds", exception.ParamName);
        }
    }
}
