
namespace Blaise.Nuget.PubSub.Contracts.Interfaces
{
    public interface IMessageHandler
    {
        bool HandleMessage(string message);
    }
}
