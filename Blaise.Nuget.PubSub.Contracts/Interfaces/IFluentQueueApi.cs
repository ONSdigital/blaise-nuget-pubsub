namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentQueueApi : IFluentSubscriptionApi, IFluentPublishApi
    {
        IFluentQueueApi ForProject(string projectId);

        IFluentQueueApi CreateTopic(string topicId);

        IFluentQueueApi CreateSubscription(string subscriptionId, int messageTimeoutInSeconds);
    }
}
