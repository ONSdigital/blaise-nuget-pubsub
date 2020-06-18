
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentSubscriptionApi WithSubscription(string subscriptionId);

        //used to throttle message flow to one message at a time
        void StartConsuming(IMessageHandler messageHandler, bool throttle = false);

        void StopConsuming();
    }
}
