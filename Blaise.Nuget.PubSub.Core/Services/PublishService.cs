using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using System.Collections.Generic;
using System.Linq;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class PublishService : IPublishService
    {
        private readonly PublisherServiceApiClient _publisherServiceClient;

        public PublishService()
        {
            _publisherServiceClient = PublisherServiceApiClient.Create();
        }

        public void PublishMessage(string projectId, string topicId, string message, Dictionary<string, string> attributes = null)
        {
            var topicName = new TopicName(projectId, topicId);
            var pubsubMessage = BuildPubsubMessage(message, attributes);

            _publisherServiceClient.Publish(topicName, new[] { pubsubMessage });
        }

        public bool TopicExists(string projectId, string topicId)
        {
            var projectName = new ProjectName(projectId);
            var topics = _publisherServiceClient.ListTopics(projectName);

            return topics.Any(t => t.TopicName.TopicId == topicId);
        }

        public void CreateTopic(string projectId, string topicId)
        {
            if(TopicExists(projectId, topicId))
            {
                return;
            }

            var topicName = new TopicName(projectId, topicId);
            _publisherServiceClient.CreateTopic(topicName);
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

        private PubsubMessage BuildPubsubMessage(string message, Dictionary<string, string> attributes)
        {
            if (attributes == null)
            {
                return new PubsubMessage
                {
                    Data = ByteString.CopyFromUtf8(message),
                };
            }

            return new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(message),
                Attributes =
                {
                    attributes
                }
            };
        }
    }
}
