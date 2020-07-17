
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentSubscriptionApi WithSubscription(string subscriptionId);

        IFluentQueueApi CreateSubscription(string subscriptionId, int ackTimeoutInSeconds = 600);

        IFluentQueueApi WithRetryPolicy(int maximumDeliveryAttempts = 5, int minimumBackOffInSeconds = 10, int maximumBackOffInSeconds = 600);

        //used to throttle message flow to one message at a time
        void StartConsuming(IMessageHandler messageHandler, bool throttle = false);

        void StopConsuming();
    }
}
