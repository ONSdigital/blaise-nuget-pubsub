# blaise-nuget-pubsub

The idea behind this repository was to provide an API abstraction of PubSub that could be produced as a Nuget package and shared across our C# repositories.

# Setup Development Environment

Clone the git repository to your IDE of choice. Visual Studio 2019 is recommended.

In order to run the behavioural tests, you will need to populate the key values in the appsettings.json file according to your local RabbitMq configuration. **Never commit App.config with key values.**

# Concepts

This solution utilizes concepts from the SOLID principles of development. In order to facilitate unit testing, dependency injection has been used throughout the code base. However, as this will be presented as a NuGet package 
  

# Use

TBC