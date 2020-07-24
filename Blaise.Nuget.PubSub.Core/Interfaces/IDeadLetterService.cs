using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface IDeadLetterService
    {
        Subscription UpdateSubscriptionWithDeadLetter(string projectId, string subscriptionId, string deadLetterTopicId, 
            int maximumDeliveryAttempts);
    }
}