﻿using Blaise.Nuget.PubSub.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.PubSub.V1;
using Nito.AsyncEx.Synchronous;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class SubscriberService : ISubscriberService
    {
        private SubscriberClient _subscriberClient;

        public void StartConsuming(string projectId, string subscriptionId, IMessageHandler messageHandler)
        {
            var createSubscriptionTask = StartConsumingAsync(projectId, subscriptionId, messageHandler);
            createSubscriptionTask.WaitAndUnwrapException();
        }

        public void StopConsuming()
        {
            var cancelSubscriptionTask = StopConsumingAsync();
            cancelSubscriptionTask.WaitAndUnwrapException();
        }

        private async Task StartConsumingAsync(string projectId, string subscriptionId, IMessageHandler messageHandler)
        {
            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            _subscriberClient = await SubscriberClient.CreateAsync(subscriptionName);

            //Blame google
            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _subscriberClient.StartAsync((msg, cancellationToken) =>
            {               
                var message = msg.Data.ToStringUtf8();

                return Task.FromResult(messageHandler.HandleMessage(message) 
                    ? SubscriberClient.Reply.Ack 
                    : SubscriberClient.Reply.Nack);

            }).ConfigureAwait(false);
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task StopConsumingAsync()
        {
            if(_subscriberClient == null)
            {
                throw new InvalidOperationException("No subscriptions have been setup");
            }

            await _subscriberClient.StopAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}


