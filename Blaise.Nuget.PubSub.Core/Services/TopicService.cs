using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using System.Linq;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class TopicService : ITopicService
    {
        private  PublisherServiceApiClient _publisherServiceClient;


        public Topic CreateTopic(string projectId, string topicId)
        {
            if (TopicExists(projectId, topicId))
            {
                return GetTopic(projectId, topicId);
            }

            var client = GetPublisherClient();
            var topicName = new TopicName(projectId, topicId);
            return client.CreateTopic(topicName);
        }


        public void DeleteTopic(string projectId, string topicId)
        {
            if (!TopicExists(projectId, topicId))
            {
                return;
            }

            var client = GetPublisherClient();
            var topicName = new TopicName(projectId, topicId);

            client.DeleteTopic(topicName);
        }

        public Topic GetTopic(string projectId, string topicId)
        {
            var client = GetPublisherClient();

            return client.GetTopic(new TopicName(projectId, topicId));
        }

        public bool TopicExists(string projectId, string topicId)
        {
            var client = GetPublisherClient();
            var projectName = new ProjectName(projectId);
            var topics = client.ListTopics(projectName);
            
            return topics.Any(t => t.TopicName.TopicId == topicId);
        }

        private PublisherServiceApiClient GetPublisherClient()
        {
            if (_publisherServiceClient == null)
            {
                _publisherServiceClient = PublisherServiceApiClient.Create();
            }

            return _publisherServiceClient;
        }
    }
}
