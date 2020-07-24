using System;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class ExponentialBackOffService : IExponentialBackOffService
    {
        private readonly ISubscriptionService _subscriptionService;

        public ExponentialBackOffService(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public Subscription UpdateSubscriptionWithExponentialBackOff(string projectId, string subscriptionId, int minimumBackOffInSeconds, 
            int maximumBackOffInSeconds)
        {
            if (!_subscriptionService.SubscriptionExists(projectId, subscriptionId))
            {
                throw new Exception();
            }

            var subscription = _subscriptionService.GetSubscription(projectId, subscriptionId);

            //add the exponential back off policy to the subscription
            AddExponentialBackOffToSubscription(subscription, minimumBackOffInSeconds, maximumBackOffInSeconds);

            //update the original subscription with the retry and deadletter changes
            return _subscriptionService.UpdateSubscription(subscription, Subscription.RetryPolicyFieldNumber);
        }

        private static void AddExponentialBackOffToSubscription(Subscription subscription, int minimumBackOffInSeconds, int maximumBackOffInSeconds)
        {
            subscription.RetryPolicy = new RetryPolicy
            {
                MinimumBackoff = new Duration { Seconds = minimumBackOffInSeconds },
                MaximumBackoff = new Duration { Seconds = maximumBackOffInSeconds }
            };
        }
    }
}