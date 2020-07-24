using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface IExponentialBackOffService
    {
        Subscription UpdateSubscriptionWithExponentialBackOff(string projectId, string subscriptionId, int minimumBackOffInSeconds,
            int maximumBackOffInSeconds);
    }
}