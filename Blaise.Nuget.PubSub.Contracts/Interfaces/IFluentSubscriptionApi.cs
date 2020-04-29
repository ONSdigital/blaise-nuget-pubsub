
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentSubscriptionApi ForSubscription(string subscriptionId, int ackDeadlineInSeconds = 60);

        void StartConsuming(IMessageHandler messageHandler);

        void StopConsuming();
    }
}
