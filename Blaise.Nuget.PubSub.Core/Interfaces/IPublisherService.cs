using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface IPublisherService
    {
        void PublishMessage(string projectId, string topicId, string message, Dictionary<string, string> attributes = null);
    }
}
