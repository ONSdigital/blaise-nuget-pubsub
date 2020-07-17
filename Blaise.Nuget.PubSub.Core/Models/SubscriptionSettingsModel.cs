using System;

namespace Blaise.Nuget.PubSub.Core.Models
{
    public class SubscriptionSettingsModel
    {
        private int _ackTimeoutInSeconds;

        public SubscriptionSettingsModel()
        {
            AckTimeoutInSeconds = 600;
        }

        public int AckTimeoutInSeconds
        {
            get => _ackTimeoutInSeconds;

            set
            {
                if (value < 10 || value > 600)
                {
                    throw new ArgumentOutOfRangeException(nameof(AckTimeoutInSeconds), "The deadline for acking messages must be between '10' and '600'");
                }

                _ackTimeoutInSeconds = value;
            }
        }

        public void SetRetrySettings(int maximumDeliveryAttempts, int minimumBackOffInSeconds, int maximumBackOffInSeconds)
        {
            RetrySettings = new RetrySettingsModel(maximumDeliveryAttempts, minimumBackOffInSeconds, maximumBackOffInSeconds);
        }

        public RetrySettingsModel RetrySettings { get; private set; }
    }
}
