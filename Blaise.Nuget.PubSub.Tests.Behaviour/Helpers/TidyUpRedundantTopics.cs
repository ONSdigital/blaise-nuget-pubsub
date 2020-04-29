using Blaise.Nuget.PubSub.Core.Services;
using System.Collections.Generic;
namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public class TidyUpRedundantTopics
    {
        public TidyUpRedundantTopics()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }
     
        public void Given_A_List_Of_Topics_These_Are_Removed_From_The_Project_In_Gcp()
        {
            //arrange
            var projectId = "ons-blaise-dev";
            var topicIds = new List<string>
            {
                "blaise-nuget-topic-24e3cb88-9022-4efb-9924-b68fb3390388",
                "blaise-nuget-topic-c7c3544d-a404-4c0e-9ae0-b6d4b24ba9cd",
                "blaise-nuget-topic-e33305ac-8f33-474b-86ad-66de53db6c45"
            };

            var topicsService = new TopicService();

            //act
            foreach(var topicId in topicIds)
            {
                topicsService.DeleteTopic(projectId, topicId);
            }
        }
    }
}
