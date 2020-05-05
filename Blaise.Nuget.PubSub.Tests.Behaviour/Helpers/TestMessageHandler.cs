using Blaise.Nuget.PubSub.Contracts.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class TestMessageHandler : IMessageHandler
    {        
        public List<string> MessagesHandled { get; set; }

        private bool _result;
        private int _delayInMilliseconds;

        public TestMessageHandler()
        {
            MessagesHandled = new List<string>();
            _result = true;
            _delayInMilliseconds = 0;
        }

        public void SetResult(bool result)
        {
            _result = result;
        }

        public void SetDelay(int delayInMilliseconds)
        {
            _delayInMilliseconds = delayInMilliseconds;
        }

        public bool HandleMessage(string message)
        {
            MessagesHandled.Add(message);

            if (_delayInMilliseconds > 0)
            {
                Thread.Sleep(_delayInMilliseconds);
            }

            return _result;
        }
    }
}
