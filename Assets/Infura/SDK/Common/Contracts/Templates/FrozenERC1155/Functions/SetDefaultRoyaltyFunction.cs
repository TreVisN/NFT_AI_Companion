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
    public partial class SetDefaultRoyaltyFunction : SetDefaultRoyaltyFunctionBase { }

    [Function("setDefaultRoyalty")]
    public class SetDefaultRoyaltyFunctionBase : FunctionMessage
    {
        [Parameter("address", "receiver", 1)]
        public virtual string Receiver { get; set; }
        [Parameter("uint96", "feeNumerator", 2)]
        public virtual BigInteger FeeNumerator { get; set; }
    }
}
