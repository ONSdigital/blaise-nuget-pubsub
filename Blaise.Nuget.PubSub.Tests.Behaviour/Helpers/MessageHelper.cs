
using Google.Cloud.PubSub.V1;
using System.Linq;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class MessageHelper
    {
        public string GetMessage(string projectId, string subscriptionId)
        {
            var subscriberService = SubscriberServiceApiClient.Create();
            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            var response = subscriberService.Pull(subscriptionName, returnImmediately: true, maxMessages: 1);

            var receivedMessage = response.ReceivedMessages.FirstOrDefault();

            if(receivedMessage == null)
            {
                return null;
            }

            subscriberService.Acknowledge(subscriptionName, new[] { receivedMessage.AckId });

            return receivedMessage.Message.Data.ToStringUtf8();
        }
    }
}
