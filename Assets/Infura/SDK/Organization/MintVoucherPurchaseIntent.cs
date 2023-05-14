using System;
using System.Numerics;
using Newtonsoft.Json;

namespace Infura.SDK.Organization
{
    public class MintVoucherPurchaseIntent
    {
        public class VoucherData
        {
            [JsonProperty("net_recipient")]
            public string NetRecipient { get; set; }
            
            [JsonProperty("initial_recipient")]
            public string InitialRecipient { get; set; }
            
            [JsonProperty("initial_recipient_amount")]
            public BigInteger InitialRecipientAmount { get; set; }
            
            [JsonProperty("quantity")]
            public int Quantity { get; set; }
            
            [JsonProperty("nonce")]
            public int Nonce { get; set; }
            
            [JsonProperty("expiry")]
            public long Expiry { get; set; }
            
            [JsonProperty("price")]
            public BigInteger Price { get; set; }
            
            [JsonProperty("token_id")]
            public BigInteger TokenId { get; set; }
            
            [JsonProperty("currency")]
            public string CurrencyAddress { get; set; }

            [JsonIgnore]
            public bool IsNativeCurrency
            {
                get
                {
                    return CurrencyAddress == "0x0000000000000000000000000000000000000000";
                }
            }
        }
        
        public class MintVoucherData
        {
            [JsonProperty("contract")]
            public string Contract { get; set; }
            
            [JsonProperty("minter")]
            public string Minter { get; set; }
            
            [JsonProperty("network_id")]
            public int NetworkId { get; set; }
            
            [JsonProperty("signature")]
            public string Signature { get; set; }
            
            [JsonProperty("voucher")]
            public VoucherData Voucher { get; set; }
        }
        
        [JsonProperty("data")]
        public MintVoucherData Data { get; set; }
        
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
        
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}