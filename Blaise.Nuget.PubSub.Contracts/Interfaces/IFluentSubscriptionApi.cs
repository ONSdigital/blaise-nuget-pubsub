
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentSubscriptionApi WithSubscription(string subscriptionId);

        IFluentQueueApi CreateSubscription(string subscriptionId, int messageTimeoutInSeconds);

        //used to throttle message flow to one message at a time
        void StartConsuming(IMessageHandler messageHandler, bool throttle = false);

        void StopConsuming();
    }
}
