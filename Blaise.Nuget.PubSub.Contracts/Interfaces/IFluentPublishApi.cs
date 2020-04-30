using System.Collections.Generic;
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentPublishApi
    {
        void Publish(string message, Dictionary<string, string> attributes = null);
    }
}
