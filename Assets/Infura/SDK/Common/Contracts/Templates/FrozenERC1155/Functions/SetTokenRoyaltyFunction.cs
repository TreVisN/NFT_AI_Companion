using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Truffle.Data;

namespace Truffle.Functions
{
    public partial class SetTokenRoyaltyFunction : SetTokenRoyaltyFunctionBase { }

    [Function("setTokenRoyalty")]
    public class SetTokenRoyaltyFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "tokenId", 1)]
        public virtual BigInteger TokenId { get; set; }
        [Parameter("address", "receiver", 2)]
        public virtual string Receiver { get; set; }
        [Parameter("uint96", "feeNumerator", 3)]
        public virtual BigInteger FeeNumerator { get; set; }
    }
}
