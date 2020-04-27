using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Core.Interfaces;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class CronExpressionService : ICronExpressionService
    {
        public string GenerateCronExpression(int intervalNumber, IntervalType intervalType)
        {
            if (intervalType == IntervalType.Minutes)
            {
                return $"0 */{intervalNumber} * ? * *";
            }

            return $"*/{intervalNumber} * * ? * *";
        }
    }
}
