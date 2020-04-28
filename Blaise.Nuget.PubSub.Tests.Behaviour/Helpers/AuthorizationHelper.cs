using System;
using System.Configuration;

namespace Blaise.Nuget.PubSub.Tests.Behaviour.Helpers
{
    public static class AuthorizationHelper
    {
        public static void SetupGoogleAuthCredentials()
        {
            var credentialKey = ConfigurationManager.AppSettings["GOOGLE_APPLICATION_CREDENTIALS"];

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialKey);

        }
    }
}
