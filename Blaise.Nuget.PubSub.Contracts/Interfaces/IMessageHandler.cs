using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IMessageHandler
    {
        bool HandleMessage(string messageType, string message);
    }
}
