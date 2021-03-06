﻿using Blaise.Nuget.PubSub.Contracts.Interfaces;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class TestMessageHandler : IMessageHandler
    {
        public List<string> MessagesHandled { get; set; }

        public List<string> MessagesNotHandled { get; set; }

        private bool _result;

        public TestMessageHandler()
        {
            MessagesHandled = new List<string>();
            MessagesNotHandled = new List<string>();
            _result = true;
        }

        public void SetResult(bool result)
        {
            _result = result;
        }

        public bool HandleMessage(string message)
        {
            if (_result)
            {
                MessagesHandled.Add(message);
            }
            else
            {
                MessagesNotHandled.Add(message);
            }

            return _result;
        }
    }
}