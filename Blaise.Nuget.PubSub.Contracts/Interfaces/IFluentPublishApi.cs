using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentPublishApi
    {
        IFluentPublishApi ForTopic(string topicId);
        void Publish(string message, Dictionary<string, string> attributes = null);
    }
}
