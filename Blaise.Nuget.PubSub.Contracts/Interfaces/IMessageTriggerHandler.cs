

namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IMessageTriggerHandler
    {
        void HandleMessage(string message);
    }
}
