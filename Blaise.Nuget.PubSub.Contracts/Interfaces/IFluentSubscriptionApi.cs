
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentSubscriptionApi ForSubscription(string subscriptionId);

        //blocking call
        void StartConsuming(IMessageHandler messageHandler);

        void StopConsuming();
    }
}
