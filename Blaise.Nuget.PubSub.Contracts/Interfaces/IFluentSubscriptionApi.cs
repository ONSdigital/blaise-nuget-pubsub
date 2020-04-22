
using Blaise.Nuget.PubSub.Contracts.Enums;

namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentSubscriptionApi ForSubscription(string subscriptionId);

        IFluentSubscriptionApi Pull(int numberOfMessages, IMessageHandler messageHandler);

        void Every(int intervalNumber, IntervalType intervalType);
    }
}
