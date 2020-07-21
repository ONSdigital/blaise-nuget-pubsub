
using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISubscriptionService
    {
        Subscription CreateSubscription(string projectId, string topicId, string subscriptionId,
            int ackTimeoutInSeconds);

        Subscription GetSubscription(string projectId, string subscriptionId);

        bool SubscriptionExists(string projectId, string subscriptionId);

        Subscription UpdateSubscription(Subscription subscription, int fieldMaskNumber);
    }
}
