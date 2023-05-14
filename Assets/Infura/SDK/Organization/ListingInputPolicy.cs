using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Infura.SDK.Organization
{
    /// <summary>
    /// InputListingContextPolicy
    /// </summary>
    [DataContract(Name = "InputListingContext_policy")]
    public class ListingInputPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListingInputPolicy" /> class.
        /// </summary>
        /// <param name="emailAddresses">emailAddresses.</param>
        /// <param name="emailClaimDuration">emailClaimDuration.</param>
        /// <param name="ethAddresses">ethAddresses.</param>
        /// <param name="itemAssignmentStrategy">itemAssignmentStrategy (default to &quot;null&quot;).</param>
        /// <param name="maxPerUser">maxPerUser.</param>
        /// <param name="paymentSessionDuration">paymentSessionDuration.</param>
        /// <param name="snapshotId">snapshotId.</param>
        /// <param name="txPayer">txPayer (default to &quot;SELLER&quot;).</param>
        /// <param name="type">SafeString (default to &quot;FCFS&quot;).</param>
        public ListingInputPolicy(List<string> emailAddresses = default(List<string>), int? emailClaimDuration = default(int?), List<string> ethAddresses = default(List<string>), string itemAssignmentStrategy = "null", int? maxPerUser = default(int?), ListingInputPolicyPaymentSessionDuration paymentSessionDuration = default(ListingInputPolicyPaymentSessionDuration), Guid? snapshotId = default(Guid?), string txPayer = "SELLER", string type = "FCFS")
        {
            this.EmailAddresses = emailAddresses;
            this.EmailClaimDuration = emailClaimDuration;
            this.EthAddresses = ethAddresses;
            // use default value if no "itemAssignmentStrategy" provided
            this.ItemAssignmentStrategy = itemAssignmentStrategy ?? "null";
            this.MaxPerUser = maxPerUser;
            this.PaymentSessionDuration = paymentSessionDuration;
            this.SnapshotId = snapshotId;
            // use default value if no "txPayer" provided
            this.TxPayer = txPayer ?? "SELLER";
            // use default value if no "type" provided
            this.Type = type ?? "FCFS";
        }

        /// <summary>
        /// Gets or Sets EmailAddresses
        /// </summary>
        [DataMember(Name = "email_addresses", EmitDefaultValue = true)]
        public List<string> EmailAddresses { get; set; }

        /// <summary>
        /// Gets or Sets EmailClaimDuration
        /// </summary>
        [DataMember(Name = "email_claim_duration", EmitDefaultValue = true)]
        public int? EmailClaimDuration { get; set; }

        /// <summary>
        /// Gets or Sets EthAddresses
        /// </summary>
        [DataMember(Name = "eth_addresses", EmitDefaultValue = true)]
        public List<string> EthAddresses { get; set; }

        /// <summary>
        /// Gets or Sets ItemAssignmentStrategy
        /// </summary>
        [DataMember(Name = "item_assignment_strategy", EmitDefaultValue = true)]
        public string ItemAssignmentStrategy { get; set; }

        /// <summary>
        /// Gets or Sets MaxPerUser
        /// </summary>
        [DataMember(Name = "max_per_user", EmitDefaultValue = true)]
        public int? MaxPerUser { get; set; }

        /// <summary>
        /// Gets or Sets PaymentSessionDuration
        /// </summary>
        [DataMember(Name = "payment_session_duration", EmitDefaultValue = true)]
        public ListingInputPolicyPaymentSessionDuration PaymentSessionDuration { get; set; }

        /// <summary>
        /// Gets or Sets SnapshotId
        /// </summary>
        [DataMember(Name = "snapshot_id", EmitDefaultValue = true)]
        public Guid? SnapshotId { get; set; }

        /// <summary>
        /// Gets or Sets TxPayer
        /// </summary>
        [DataMember(Name = "tx_payer", EmitDefaultValue = true)]
        public string TxPayer { get; set; }

        /// <summary>
        /// SafeString
        /// </summary>
        /// <value>SafeString</value>
        [DataMember(Name = "type", EmitDefaultValue = true)]
        public string Type { get; set; }

    }
}