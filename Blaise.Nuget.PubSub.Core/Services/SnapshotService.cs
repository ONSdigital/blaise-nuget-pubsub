using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SnapshotService : ISnapshotService
    {
        private SubscriberServiceApiClient _subscriberServiceClient;

        public SnapshotService(SubscriberServiceApiClient subscriberServiceClient)
        {
            _subscriberServiceClient = subscriberServiceClient;
        }

        public void CreateSnapshot()
        {

        }
    }
}
