using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Infura.SDK.Organization
{
    /// <summary>
    /// InputListingContextTokenRangesInner
    /// </summary>
    [DataContract(Name = "InputListingContext_token_ranges_inner")]
    public class ListingInputTokenRanges
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListingInputTokenRanges" /> class.
        /// </summary>
        [JsonConstructor]
        protected ListingInputTokenRanges() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ListingInputTokenRanges" /> class.
        /// </summary>
        /// <param name="fromId">Uint256 (required).</param>
        /// <param name="toId">Uint256 (required).</param>
        public ListingInputTokenRanges(string fromId = default(string), string toId = default(string))
        {
            // to ensure "fromId" is required (not null)
            if (fromId == null)
            {
                throw new ArgumentNullException("fromId is a required property for InputListingContextTokenRangesInner and cannot be null");
            }
            this.FromId = fromId;
            // to ensure "toId" is required (not null)
            if (toId == null)
            {
                throw new ArgumentNullException("toId is a required property for InputListingContextTokenRangesInner and cannot be null");
            }
            this.ToId = toId;
        }

        /// <summary>
        /// Uint256
        /// </summary>
        /// <value>Uint256</value>
        [DataMember(Name = "from_id", IsRequired = true, EmitDefaultValue = true)]
        public string FromId { get; set; }

        /// <summary>
        /// Uint256
        /// </summary>
        /// <value>Uint256</value>
        [DataMember(Name = "to_id", IsRequired = true, EmitDefaultValue = true)]
        public string ToId { get; set; }
    }
}