using Blaise.Nuget.PubSub.Core.Models;
using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface IDeadLetterSubscriptionService
    {
        Subscription CreateSubscriptionWithDeadLetter(string projectId, string topicId, string subscriptionId,
            int ackTimeoutInSeconds, RetrySettingsModel settingsModel);
    }
}