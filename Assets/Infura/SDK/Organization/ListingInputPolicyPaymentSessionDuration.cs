using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Infura.SDK.Organization
{
    /// <summary>
    /// EditListingInputPolicyPaymentSessionDuration
    /// </summary>
    [DataContract(Name = "EditListingInput_policy_payment_session_duration")]
    public class ListingInputPolicyPaymentSessionDuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListingInputPolicyPaymentSessionDuration" /> class.
        /// </summary>
        /// <param name="providerOverride">providerOverride.</param>
        /// <param name="timeoutSeconds">timeoutSeconds.</param>
        public ListingInputPolicyPaymentSessionDuration(Dictionary<string, int> providerOverride = default(Dictionary<string, int>), int? timeoutSeconds = default(int?))
        {
            this.ProviderOverride = providerOverride;
            this.TimeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// Gets or Sets ProviderOverride
        /// </summary>
        [DataMember(Name = "provider_override", EmitDefaultValue = true)]
        public Dictionary<string, int> ProviderOverride { get; set; }

        /// <summary>
        /// Gets or Sets TimeoutSeconds
        /// </summary>
        [DataMember(Name = "timeout_seconds", EmitDefaultValue = true)]
        public int? TimeoutSeconds { get; set; }
    }
}