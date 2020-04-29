
namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ISubscriptionService
    {
        void CreateSubscription(string projectId, string topicId, string subscriptionId, int ackDeadlineInSeconds);
    }
}
