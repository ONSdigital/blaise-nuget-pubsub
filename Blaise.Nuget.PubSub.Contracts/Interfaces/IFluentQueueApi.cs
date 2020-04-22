using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IFluentQueueApi : IFluentSubscriptionApi, IFluentPublishApi
    {
        IFluentQueueApi ForProject(string projectId);
    }
}
