using Blaise.Nuget.PubSub.Core.Services;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface IPublishService
    {
        void PublishMessage(string projectId, string topicId, string message, Dictionary<string, string> attributes = null);
    }
}
