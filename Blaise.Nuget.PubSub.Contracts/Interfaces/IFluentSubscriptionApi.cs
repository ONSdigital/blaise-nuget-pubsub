
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentSubscriptionApi ForSubscription(string subscriptionId);

        void StartConsuming(IMessageHandler messageHandler, int stopConsumingAfterSeconds = 0);

        void StopConsuming();
    }
}
