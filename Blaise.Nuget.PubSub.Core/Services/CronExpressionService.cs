using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Core.Interfaces;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class CronExpressionService : ICronExpressionService
    {
        public string GenerateCronExpression(int intervalNumber, IntervalType intervalType)
        {
            var minNumber = 1;
            var maxNumber = 59;

            if (intervalType == IntervalType.Hours)
            {
                maxNumber = 11;
            }

            if (intervalNumber < minNumber || intervalNumber > maxNumber)
            {
                throw new System.ArgumentOutOfRangeException(null,
                    $"The valid range for the type '{intervalType.ToString().ToLower()}' " +
                    $"is between {minNumber} and {maxNumber}");
            }

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
    }
}
