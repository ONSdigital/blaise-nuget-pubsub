
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentSubscriptionApi ForSubscription(string subscriptionId);

        void StartConsuming(IMessageHandler messageHandler);

        void StopConsuming();
    }
}
