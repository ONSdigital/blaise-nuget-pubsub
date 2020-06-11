using System;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class TestConcurrencyMessageHandler : IMessageHandler
    {
        public List<string> MessagesHandled { get; set; }

        public List<DateTime> TimeMessagesHandled { get; set; }

        private readonly int _delayInSeconds;

        public TestConcurrencyMessageHandler(int delayInSeconds)
        {
            MessagesHandled = new List<string>();
            TimeMessagesHandled = new List<DateTime>();

            _delayInSeconds = delayInSeconds;
        }
        
        public bool HandleMessage(string message)
        {
            {
                MessagesHandled.Add(message);
                TimeMessagesHandled.Add(DateTime.Now.ToLocalTime());

                if (_delayInSeconds > 0)
                {
                    Thread.Sleep(_delayInSeconds * 1000);
                }
            }

            return true;
        }

        public bool HandledConcurrently()
        {

            if (!TimeMessagesHandled.Any())
            {
                throw new NullReferenceException("No messages have been handled");
            }

            var firstMessageHandledSeconds = TimeMessagesHandled.First().Second;

            foreach (var timeMessageHandled in TimeMessagesHandled)
            {
                if (timeMessageHandled.Second != firstMessageHandledSeconds)
                {
                    return false;
                }
            }

            return true;
        }
    }
}