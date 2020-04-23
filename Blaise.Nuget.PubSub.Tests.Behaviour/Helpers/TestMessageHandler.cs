using Blaise.Nuget.PubSub.Contracts.Interfaces;
using System;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class TestMessageHandler : IMessageHandler
    {        
        public List<string> MessagesHandled { get; set; }

        public TestMessageHandler()
        {
            MessagesHandled = new List<string>();
        }

        public bool HandleMessage(string message)
        {
            MessagesHandled.Add(message);

            return true;
        }
    }
}
