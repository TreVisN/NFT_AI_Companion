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
    public partial class CancelVoucherFunction : CancelVoucherFunctionBase { }

    [Function("cancelVoucher")]
    public class CancelVoucherFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "voucherNonce", 1)]
        public virtual BigInteger VoucherNonce { get; set; }
    }
}
