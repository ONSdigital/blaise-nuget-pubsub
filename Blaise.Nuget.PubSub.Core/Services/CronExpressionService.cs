using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Core.Interfaces;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class CronExpressionService : ICronExpressionService
    {
        public string GenerateCronExpression(int intervalNumber, IntervalType intervalType)
        {
            if (intervalNumber < 0)
            {
                throw new System.ArgumentOutOfRangeException(null,
                    "The valid range for the type 'seconds' is between 1 and 59");
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
