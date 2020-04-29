using Blaise.Nuget.PubSub.Core.Services;
using Blaise.Nuget.PubSub.Tests.Behaviour.Helpers;
using NUnit.Framework;
namespace Blaise.Nuget.PubSub.Tests.Behaviour.Services
{
    public class PublisherServiceTests
    {
        private string _projectId;

        private PublisherService _sut;

        public PublisherServiceTests()
        {
            AuthorizationHelper.SetupGoogleAuthCredentials();
        }

        [SetUp]
        public void Setup()
        {
            _projectId = "ons-blaise-dev";         
            _sut = new PublisherService();            
        }

    }
}
