using System.Collections.Generic;
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentPublishApi
    {
        IFluentQueueApi ForTopic(string topicId);
        void Publish(string message, Dictionary<string, string> attributes = null);
    }
}
