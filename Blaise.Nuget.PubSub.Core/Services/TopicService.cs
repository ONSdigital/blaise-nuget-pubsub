using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using System.Linq;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class TopicService : ITopicService
    {
        private readonly PublisherServiceApiClient _publisherServiceClient;

        public TopicService()
        {
            _publisherServiceClient = PublisherServiceApiClient.Create();
        }

        public Topic CreateTopic(string projectId, string topicId)
        {
            if (TopicExists(projectId, topicId))
            {
                return GetTopic(projectId, topicId);
            }

            var topicName = new TopicName(projectId, topicId);
            return _publisherServiceClient.CreateTopic(topicName);
        }


        public void DeleteTopic(string projectId, string topicId)
        {
            if (!TopicExists(projectId, topicId))
            {
                return;
            }

            var topicName = new TopicName(projectId, topicId);
            _publisherServiceClient.DeleteTopic(topicName);
        }

        public Topic GetTopic(string projectId, string topicId)
        {
            return _publisherServiceClient.GetTopic(new TopicName(projectId, topicId));
        }

        public bool TopicExists(string projectId, string topicId)
        {
            var projectName = new ProjectName(projectId);
            var topics = _publisherServiceClient.ListTopics(projectName);
            
            return topics.Any(t => t.TopicName.TopicId == topicId);
        }
    }
}
