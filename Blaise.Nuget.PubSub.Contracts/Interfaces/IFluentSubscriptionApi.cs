
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentQueueApi WithSubscription(string subscriptionId);

        IFluentQueueApi CreateSubscription(string subscriptionId, int ackTimeoutInSeconds = 600);

        IFluentQueueApi WithExponentialBackOff(int minimumBackOffInSeconds = 10, int maximumBackOffInSeconds = 600);

        IFluentQueueApi WithDeadLetter(string serviceAccountName, string deadLetterTopicId, int maximumDeliveryAttempts = 5);

        //Throttle is used to control message flow to one message at a time
        void StartConsuming(IMessageHandler messageHandler, bool throttle = false); 

        void StopConsuming();
    }
}
