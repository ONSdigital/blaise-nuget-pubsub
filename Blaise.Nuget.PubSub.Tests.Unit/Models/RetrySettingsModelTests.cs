using System;
using Blaise.Nuget.PubSub.Core.Models;
using NUnit.Framework;

namespace Blaise.Nuget.PubSub.Tests.Unit.Models
{
    public class RetrySettingsModelTests
    {
        private readonly string _serviceAccountName;
        private readonly int _maximumBackOffInSeconds;
        private readonly int _minimumBackOffInSeconds;
        private readonly int _maximumDeliveryAttempts;

        public RetrySettingsModelTests()
        {
            _serviceAccountName = "ServiceAccount1";
            _maximumBackOffInSeconds = 10;
            _minimumBackOffInSeconds = 10;
            _maximumDeliveryAttempts = 5;
        }


        [Test]
        public void Given_Valid_Values_When_I_Set_The_Value_Then_An_Exception_Is_Not_Thrown()
        {
            //act && assert
            Assert.DoesNotThrow(() => new RetrySettingsModel(_serviceAccountName, _maximumDeliveryAttempts,
                _minimumBackOffInSeconds, _maximumBackOffInSeconds));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Given_Invalid_ServiceAccountName_When_I_Set_The_Value_Then_An_ArgumentException_Is_Thrown(string serviceAccountName)
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => new RetrySettingsModel(serviceAccountName, _maximumDeliveryAttempts,
                _minimumBackOffInSeconds, _maximumBackOffInSeconds));
            Assert.AreEqual("You must provide a valid service account name", exception.ParamName);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(11)]
        [TestCase(20)]
        public void Given_Invalid_MaximumDeliveryAttempts_When_I_Set_The_Value_Then_An_ArgumentOutOfRangeException_Is_Thrown(int maximumDeliveryAttempts)
        {

            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new RetrySettingsModel(_serviceAccountName, maximumDeliveryAttempts,
                _minimumBackOffInSeconds, _maximumBackOffInSeconds));
            Assert.AreEqual("MaximumDeliveryAttempts", exception.ParamName);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(601)]
        public void Given_Invalid_MinimumBackOffInSeconds_When_I_Set_The_Value_Then_An_ArgumentOutOfRangeException_Is_Thrown(int minimumBackOffInSeconds)
        {

            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new RetrySettingsModel(_serviceAccountName, _maximumDeliveryAttempts,
                minimumBackOffInSeconds, _maximumBackOffInSeconds));
            Assert.AreEqual("MinimumBackOffInSeconds", exception.ParamName);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(601)]
        public void Given_Invalid_MaximumBackOffInSeconds_When_I_Set_The_Value_Then_An_ArgumentOutOfRangeException_Is_Thrown(int maximumBackOffInSeconds)
        {

            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new RetrySettingsModel(_serviceAccountName, _maximumDeliveryAttempts,
                _minimumBackOffInSeconds, maximumBackOffInSeconds));
            Assert.AreEqual("MaximumBackOffInSeconds", exception.ParamName);
        }
    }
}
