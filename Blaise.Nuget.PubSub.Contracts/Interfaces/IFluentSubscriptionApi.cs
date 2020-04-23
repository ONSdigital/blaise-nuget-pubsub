
using Blaise.Nuget.PubSub.Contracts.Enums;

namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentSubscriptionApi
    {
        IFluentSubscriptionApi ForSubscription(string subscriptionId);

        IFluentSubscriptionApi Consume(int numberOfMessages, IMessageHandler messageHandler);

        void Now();

        void Every(int intervalNumber, IntervalType intervalType);
    }
}
