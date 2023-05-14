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
    public partial class BalanceOfBatchFunction : BalanceOfBatchFunctionBase { }

    [Function("balanceOfBatch", "uint256[]")]
    public class BalanceOfBatchFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "accounts", 1)]
        public virtual List<string> Accounts { get; set; }
        [Parameter("uint256[]", "ids", 2)]
        public virtual List<BigInteger> Ids { get; set; }
    }
}
