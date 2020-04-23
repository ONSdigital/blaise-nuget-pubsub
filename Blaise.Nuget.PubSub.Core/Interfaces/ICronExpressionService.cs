using Blaise.Nuget.PubSub.Contracts.Enums;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ICronExpressionService
    {
        string GenerateCronExpression(int intervalNumber, IntervalType intervalType);
    }
}
