using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Blaise.Nuget.PubSub.Core.Models;
using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISchedulerService _schedulerService;

        public SubscriptionService(ISchedulerService schedulerService)
        {
            _schedulerService = schedulerService;
        }

        public void Subscribe(string projectId, string subscriptionId, IMessageHandler messageHandler, ScheduleModel scheduleModel)
        {
            var subscriberService = SubscriberServiceApiClient.Create();
            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            var response = subscriberService.Pull(subscriptionName, returnImmediately: true, maxMessages: scheduleModel.NumberOfMessages);

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
