using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.PubSub.V1;
using System;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        public void Consume(string projectId, string subscriptionId, int numberOfMessages, IMessageHandler messageHandler)
        {
            var subscriberService = SubscriberServiceApiClient.Create();
            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            var response = subscriberService.Pull(subscriptionName, returnImmediately: true, maxMessages: numberOfMessages);

            Console.WriteLine($"Number of messages available - {response.ReceivedMessages.Count}");

            foreach (ReceivedMessage received in response.ReceivedMessages)
            {
                var message = received.Message.Data.ToStringUtf8();

                if (messageHandler.HandleMessage(message))
                {
                    subscriberService.Acknowledge(subscriptionName, new[] { received.AckId });
                }
            };
        }
    }
}


