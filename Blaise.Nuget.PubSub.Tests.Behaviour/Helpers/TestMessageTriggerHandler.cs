using System.Collections.Generic;
using Blaise.Nuget.PubSub.Contracts.Interfaces;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class TestMessageTriggerHandler : IMessageTriggerHandler
    {
        public TestMessageTriggerHandler()
        {
            MessagesHandled = new List<string>();
        }

        public List<string> MessagesHandled { get; set; }

        public void HandleMessage(string message)
        {
            MessagesHandled.Add(message);
        }
    }
}