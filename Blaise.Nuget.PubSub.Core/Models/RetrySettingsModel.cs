
using System;

namespace Blaise.Nuget.PubSub.Core.Models
{
    public class RetrySettingsModel
    {
        private string _serviceAccountName;
        private int _maximumBackOffInSeconds;
        private int _minimumBackOffInSeconds;
        private int _maximumDeliveryAttempts;

        public RetrySettingsModel(string serviceAccountName, int maximumDeliveryAttempts, int minimumBackOffInSeconds, int maximumBackOffInSeconds)
        {
            ServiceAccountName = serviceAccountName;
            MaximumBackOffInSeconds = maximumBackOffInSeconds;
            MinimumBackOffInSeconds = minimumBackOffInSeconds;
            MaximumDeliveryAttempts = maximumDeliveryAttempts;
        }

        public string ServiceAccountName
        {
            get => _serviceAccountName;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(nameof(ServiceAccountName), $"ServiceAccountName");
                }

                _serviceAccountName = value;
            }
        }

        public int MaximumBackOffInSeconds
        {
            get => _maximumBackOffInSeconds;
            private set
            {
                if (value < 10 || value > 600)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaximumBackOffInSeconds), "The range for the minimum back off between retries is between '10' and '600'");
                }

                _maximumBackOffInSeconds = value;
            }
        }

        public int MinimumBackOffInSeconds
        {
            get => _minimumBackOffInSeconds;
            private set
            {
                if (value < 10 || value > 600)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinimumBackOffInSeconds), "The range for the minimum back off between retries is between '10' and '600'");
                }

                _minimumBackOffInSeconds = value;
            }
        }

        public int MaximumDeliveryAttempts
        {
            get => _maximumDeliveryAttempts;
            private set
            {
                if (value < 1 || value > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaximumDeliveryAttempts), "The range for the maximum number of delivery attempts is between '1' and '100'");
                }

                _maximumDeliveryAttempts = value;
            }
        }
    }
}
