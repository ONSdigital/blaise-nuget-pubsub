using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SnapshotService
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
