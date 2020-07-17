using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class DeadLetterService : IDeadLetterService
    {
        private readonly ITopicService _topicService;

        public DeadLetterService(ITopicService topicService)
        {
            _topicService = topicService;
        }

        public DeadLetterPolicy CreateDeadLetterPolicy(string projectId, string topicId, int maximumDeliveryAttempts)
        {
            var deadLetterTopicId = $"{topicId}-deadletter";
            var deadLetterTopic = _topicService.CreateTopic(projectId, deadLetterTopicId);

            return new DeadLetterPolicy
            {
                MaxDeliveryAttempts = maximumDeliveryAttempts,
                DeadLetterTopic = deadLetterTopic.Name
            };
        }
    }
}