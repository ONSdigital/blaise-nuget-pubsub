using Blaise.Nuget.PubSub.Contracts.Interfaces;

namespace Blaise.Nuget.PubSub.Tests.Unit.Helpers
{
    public class TestMessageHandler : IMessageHandler
    {        
        public bool HandleMessage(string message)
        {
            return true;
        }
    }
}
