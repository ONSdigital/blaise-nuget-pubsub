# blaise-nuget-pubsub

The idea behind this repository was to provide an API abstraction of PubSub that could be produced as a NuGet package and shared across our C# repositories.

# Setup Development Environment

Clone the git repository to your IDE of choice. Visual Studio 2019 is recommended.

In order to run the behavioural tests, you will need to populate the key values in the appsettings.json file according to your local RabbitMq configuration. **Never commit App.config with key values.**

# Concepts

This solution utilizes concepts from the SOLID principles of development. In order to facilitate unit testing, dependency injection has been used throughout the code base. However, as this will be presented as a NuGet package 
  

# Use

This repository generates two NuGet packages 'Blaise.Nuget.PubSub' and 'Blaise.Nuget.PubSub.Contracts'. The Contracts package contains the interface for the API and you can use this package in your unit test projects 
mocks where you will not need to consume the complete functionality. 

You can use the fluid API style class 'FluentQueueApi' which implements the 'IFluentQueueApi' interface.

##Subscriptions

For subscribing to a topic you use the Api to specify the projectId, the subscriptionId and provide a 'message handler' which will be used to process the messages that are published to the subscribed topic. The
message handler is a class that implements the 'IMessageHandler' interface that is contained in the 'Blaise.Nuget.PubSub.Contracts' NuGet package. This interface expects a method called 'HandleMessage' that accepts a string
for the Json message that it will retrieve from the subscription and a boolean return value. If the method returns 'true' the message will be 'acked' in the subscription and will be removed. If it returns 'false' the message will be 'nacked' 
and will remain in the subscription depending if you have set up a dead letter policy.

Please see the example below for using the Api to setup a subscription, along with an example of a messagehandler: 

	FluentQueueApi
		.ForProject('projectId')
		.ForSubscription('subscriptionId')
		.StartConsuming('messageHandler');

	public class TestMessageHandler : IMessageHandler
    {        
        public bool HandleMessage(string message)
        {
            if (MessageIsProcessed(message))
            {
                //ack is sent
                return true;
            }
            else
            {
                //nack is sent
                return false;
            }
        }
    }
		  
		 
##Publishing

To publish to a topic, you simply need to provide the projectId, topicId and a Json serialized message. Please see the example below for using the Api to publish a message to a topic:
		  
	FluentQueueApi
        .ForProject('projectId')
        .ForTopic('topicId')
        .Publish('message');
		
Copyright (c) 2019 Crown Copyright (Government Digital Service)		