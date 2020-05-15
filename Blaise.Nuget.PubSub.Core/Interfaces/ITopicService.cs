using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ITopicService
    {
        Topic CreateTopic(string projectId, string topicId);
    }
}
