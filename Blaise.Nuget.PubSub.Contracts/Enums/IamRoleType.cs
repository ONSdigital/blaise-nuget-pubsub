using System.ComponentModel;

namespace Blaise.Nuget.PubSub.Contracts.Enums
{
    public enum IamRoleType
    {
        [Description("roles/pubsub.subscriber")]
        Subscriber,

        [Description("roles/pubsub.publisher")]
        Publisher
    }
}