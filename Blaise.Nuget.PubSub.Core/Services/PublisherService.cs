using Blaise.Nuget.PubSub.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class PublisherService : IPublisherService
    {
        public void PublishMessage(string projectId, string topicId, string message, Dictionary<string, string> attributes = null)
        {
            throw new NotImplementedException();
        }
    }
}
