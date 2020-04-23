using Blaise.Nuget.PubSub.Contracts.Interfaces;
using System;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class TestScheduledMessageHandler : IMessageHandler
    {        
        public Dictionary<string, string> MessagesHandled { get; set; }

        public TestScheduledMessageHandler()
        {
            MessagesHandled = new Dictionary<string, string>();
        }

        public bool HandleMessage(string message)
        {
            MessagesHandled.Add(message, DateTime.Now.ToLongTimeString());

            return true;
        }
    }
}
