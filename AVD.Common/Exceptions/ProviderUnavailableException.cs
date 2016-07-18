using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AVD.Common.Exceptions
{
    [Description("Critical - the provider is unavailable and as such we can not process any requests with it")]
    public class ProviderUnavailableException : ProviderException
    {
        public ProviderUnavailableException(String providerName, Exception innerException)
            : base(providerName, innerException)
        {
            ProviderName = providerName;
        }

        public ProviderUnavailableException(String providerName, String developerMessage, Exception innerException)
            : base(providerName, developerMessage, innerException)
        {
            ProviderName = providerName;
        }
    }
}
