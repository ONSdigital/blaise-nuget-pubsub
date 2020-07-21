using Blaise.Nuget.PubSub.Contracts.Enums;

namespace Blaise.Nuget.PubSub.Core.Interfaces
{
    public interface IIamPolicyRequestService
    {
        void GrantPermissionsForAccount(string resourceName, string serviceAccountName,
            IamRoleType roleType);
    }
}