using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class PublisherService : IPublisherService
    {
        public void PublishMessage(string projectId, string topicId, string message, Dictionary<string, string> attributes = null)
        {
            var publisherServiceClient = PublisherServiceApiClient.Create();
            var topicName = new TopicName(projectId, topicId);
            var message = BuildMessage(message, attributes);

            publisherServiceClient.Publish(topicName, new[] { message });
        }

        private PubsubMessage BuildMessage(string message, Dictionary<string, string> attributes)
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
