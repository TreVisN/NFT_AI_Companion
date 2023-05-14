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
    public partial class MintWithVoucherFunction : MintWithVoucherFunctionBase { }

    [Function("mintWithVoucher")]
    public class MintWithVoucherFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "voucher", 1)]
        public virtual MintVoucher Voucher { get; set; }
        [Parameter("bytes", "signature", 2)]
        public virtual byte[] Signature { get; set; }
    }
}
