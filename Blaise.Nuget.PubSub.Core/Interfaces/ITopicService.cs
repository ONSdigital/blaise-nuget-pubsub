namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface ITopicService
    {
        void CreateTopic(string projectId, string topicId);
    }
}
