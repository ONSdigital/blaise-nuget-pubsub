using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Core.Services;
using NUnit.Framework;
using System;

namespace Blaise.Nuget.PubSub.Tests.Unit.Core
{
    public class CronExpressionServiceTests
    {
        [TestCase(5, IntervalType.Seconds, "*/5 * * ? * *")]
        [TestCase(10, IntervalType.Seconds, "*/10 * * ? * *")]
        [TestCase(20, IntervalType.Seconds, "*/20 * * ? * *")]
        [TestCase(30, IntervalType.Seconds, "*/30 * * ? * *")]
        [TestCase(59, IntervalType.Seconds, "*/59 * * ? * *")]
        [TestCase(2, IntervalType.Minutes, "0 */2 * ? * *")]
        [TestCase(5, IntervalType.Minutes, "0 */5 * ? * *")]
        [TestCase(10, IntervalType.Minutes, "0 */10 * ? * *")]
        [TestCase(20, IntervalType.Minutes, "0 */20 * ? * *")]
        [TestCase(30, IntervalType.Minutes, "0 */30 * ? * *")]
        [TestCase(59, IntervalType.Minutes, "0 */59 * ? * *")]
        [TestCase(2, IntervalType.Hours, "0 0 */2 ? * *")]
        [TestCase(3, IntervalType.Hours, "0 0 */3 ? * *")]
        [TestCase(6, IntervalType.Hours, "0 0 */6 ? * *")]
        public void Given_Valid_Arguments_When_I_Call_GenerateCron_I_Get_An_Expected_Cron_Expression_Back(int intervalNumber, IntervalType intervalType, string expectedCronExpression)
        {
            //arrange
            var sut = new CronExpressionService();

            //act
            var result = sut.GenerateCronExpression(intervalNumber, intervalType);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCronExpression, result);
        }

        [TestCase(-1, IntervalType.Seconds, "The valid range for the type 'seconds' is between '1' and '59'")]
        [TestCase(0, IntervalType.Seconds, "The valid range for the type 'seconds' is between '1' and '59'")]
        [TestCase(60, IntervalType.Seconds, "The valid range for the type 'seconds' is between '1' and '59'")]
        [TestCase(61, IntervalType.Seconds, "The valid range for the type 'seconds' is between '1' and '59'")]
        [TestCase(-1, IntervalType.Minutes, "The valid range for the type 'minutes' is between '1' and '59'")]
        [TestCase(0, IntervalType.Minutes, "The valid range for the type 'minutes' is between '1' and '59'")]
        [TestCase(60, IntervalType.Minutes, "The valid range for the type 'minutes' is between '1' and '59'")]
        [TestCase(61, IntervalType.Minutes, "The valid range for the type 'minutes' is between '1' and '59'")]
        [TestCase(-1, IntervalType.Hours, "The valid range for the type 'hours' is between '1' and '11'")]
        [TestCase(0, IntervalType.Hours, "The valid range for the type 'hours' is between '1' and '11'")]
        [TestCase(12, IntervalType.Hours, "The valid range for the type 'hours' is between '1' and '11'")]
        [TestCase(13, IntervalType.Hours, "The valid range for the type 'hours' is between '1' and '11'")]
        public void Given_Invalid_Arguments_When_I_Call_GenerateCron_Then_An_ArgumentOutOfRangeException_Is_Thrown(
            int intervalNumber, IntervalType intervalType, string expectedErrorMessage)
        {
            //arrange
            var sut = new CronExpressionService();

            //act && assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => sut.GenerateCronExpression(intervalNumber, intervalType));
            Assert.AreEqual(expectedErrorMessage, exception.Message);
        }
    }
}
