using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface IDeadLetterService
    {
        DeadLetterPolicy CreateDeadLetterPolicy(string projectId, string topicId, int maximumDeliveryAttempts);
    }
}