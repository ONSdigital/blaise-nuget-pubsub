﻿
using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISubscriptionService
    {
        Subscription CreateSubscription(string projectId, string topicId, string subscriptionId, int messageTimeoutInSeconds);
    }
}
