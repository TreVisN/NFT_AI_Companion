using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Truffle.Data
{
    public partial class MintVoucher : MintVoucherBase { }

    public class MintVoucherBase 
    {
        [Parameter("address", "netRecipient", 1)]
        public virtual string NetRecipient { get; set; }
        [Parameter("address", "initialRecipient", 2)]
        public virtual string InitialRecipient { get; set; }
        [Parameter("uint256", "initialRecipientAmount", 3)]
        public virtual BigInteger InitialRecipientAmount { get; set; }
        [Parameter("uint256", "quantity", 4)]
        public virtual BigInteger Quantity { get; set; }
        [Parameter("uint256", "nonce", 5)]
        public virtual BigInteger Nonce { get; set; }
        [Parameter("uint256", "expiry", 6)]
        public virtual BigInteger Expiry { get; set; }
        [Parameter("uint256", "price", 7)]
        public virtual BigInteger Price { get; set; }
        [Parameter("uint256", "tokenId", 8)]
        public virtual BigInteger TokenId { get; set; }
        [Parameter("address", "currency", 9)]
        public virtual string Currency { get; set; }
    }
}
