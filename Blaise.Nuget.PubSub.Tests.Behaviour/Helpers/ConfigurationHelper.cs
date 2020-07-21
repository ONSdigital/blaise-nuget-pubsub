
using System.Configuration;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class ConfigurationHelper
    {
        public string ProjectId => ConfigurationManager.AppSettings["ProjectId"];

        public string TopicId => ConfigurationManager.AppSettings["TopicId"];

        public string SubscriptionId => ConfigurationManager.AppSettings["SubscriptionId"];

        public string DeadletterTopicId => ConfigurationManager.AppSettings["DeadletterTopicId"];

        public string DeadletterSubscriptionId => ConfigurationManager.AppSettings["DeadletterSubscriptionId"];

        public string ServiceAccountName => ConfigurationManager.AppSettings["ServiceAccountName"];
    }
}
