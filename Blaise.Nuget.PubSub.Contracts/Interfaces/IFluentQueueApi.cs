namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentQueueApi : IFluentSubscriptionApi, IFluentPublishApi
    {
        IFluentQueueApi ForProject(string projectId);
    }
}
