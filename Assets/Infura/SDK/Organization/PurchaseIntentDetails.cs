using System;
using Newtonsoft.Json;

namespace Infura.SDK.Organization
{
    public class PurchaseIntentDetails
    {
        [JsonProperty("coinbase_charge_id")]
        public string CoinbaseChargeId { get; set; }
        
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("eth_address")]
        public string EthAddress { get; set; }
        
        [JsonProperty("expiration")]
        public DateTime Expiration { get; set; }
        
        [JsonProperty("fulfillment_method")]
        public string FulfillmentMethod { get; set; }
        
        [JsonProperty("fulfillment_status")]
        public string FulfillmentStatus { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("item_claim_ids")]
        public string[] ItemClaimIds { get; set; }
        
        [JsonProperty("listing_id")]
        public string ListingId { get; set; }
        
        [JsonProperty("payment_provider")]
        public string PaymentProvider { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("stripe_payment_intent_id")]
        public string StripePaymentIntentId { get; set; }
        
        [JsonProperty("transaction_ids")]
        public string[] TransactionIds { get; set; }
    }
}