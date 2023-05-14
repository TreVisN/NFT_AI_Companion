using Newtonsoft.Json;

namespace Infura.SDK.Organization
{
    public class PurchaseIntentRequest
    {
        public class PurchaseIntentBuyer
        {
            [JsonProperty("eth_address")]
            public string EthAddress { get; set; }
            
            [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
            public string Email { get; set; }
        }
        
        [JsonProperty("provider")]
        public string Provider { get; set; }
        
        [JsonProperty("listing_id")]
        public string ListingId { get; set; }
        
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        
        [JsonProperty("buyer")]
        public PurchaseIntentBuyer Buyer { get; set; }
    }
}