
using System;
using System.Collections.Generic;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class TestSchedulerAction
    {        
        public List<double> ActionsIntervalLogged { get; set; }

        public TestSchedulerAction()
        {
            ActionsIntervalLogged = new List<double>();
        }

        public bool LogAction(DateTime scheduled)
        {
            var interval = ActionInterval(scheduled, DateTime.Now);
            ActionsIntervalLogged.Add(interval);

            return true;
        }

        private double ActionInterval(DateTime scheduled, DateTime actioned)
        {
            return (actioned - scheduled).TotalSeconds;
        }
    }
}
