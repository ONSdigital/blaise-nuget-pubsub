using Blaise.Nuget.PubSub.Contracts.Enums;
using Blaise.Nuget.PubSub.Core.Extensions;
using Blaise.Nuget.PubSub.Core.Interfaces;
using Google.Cloud.Iam.V1;
using Google.Cloud.PubSub.V1;

namespace Blaise.Nuget.PubSub.Core.Services
{
    public class IamPolicyRequestService : IIamPolicyRequestService
    {
        public void GrantPermissionsForAccount(string resourceName, string serviceAccountName, IamRoleType roleType)
        {
            var publisherServiceClient = PublisherServiceApiClient.Create();

            var subscriptionRequest = new SetIamPolicyRequest
            {
                Resource = resourceName,
                Policy = new Policy
                {
                    Bindings =
                    {
                        new Binding {
                            Role = roleType.FromDescription(),
                            Members = { $"serviceAccount:{serviceAccountName}@gcp-sa-pubsub.iam.gserviceaccount.com" } }
                    }
                }
            };

            publisherServiceClient.SetIamPolicy(subscriptionRequest);
        }
    }
}
