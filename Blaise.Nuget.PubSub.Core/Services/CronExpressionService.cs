using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Core.Interfaces;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class CronExpressionService : ICronExpressionService
    {
        public string GenerateCronExpression(int intervalNumber, IntervalType intervalType)
        {
            ValidateIntervalValuesAreInRange(intervalNumber, intervalType);

            if (intervalType == IntervalType.Seconds)
            {
                return $"*/{intervalNumber} * * ? * *";
            }
            
            if (intervalType == IntervalType.Minutes)
            {
                return $"0 */{intervalNumber} * ? * *";
            }
            
            return $"0 0 */{intervalNumber} ? * *";
        }

        private void ValidateIntervalValuesAreInRange(int intervalNumber, IntervalType intervalType)
        {
            var minNumber = 1;
            var maxNumber = intervalType == IntervalType.Hours ? 11 : 59;

            if (intervalNumber < minNumber || intervalNumber > maxNumber)
            {
                throw new System.ArgumentOutOfRangeException(null,
                    $"The valid range for the type '{intervalType.ToString().ToLower()}' " +
                    $"is between '{minNumber}' and '{maxNumber}'");
            }
        }
    }
}
