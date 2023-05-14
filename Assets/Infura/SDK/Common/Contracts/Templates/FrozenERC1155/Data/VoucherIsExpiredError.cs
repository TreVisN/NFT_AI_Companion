using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Truffle.Data
{
    public partial class VoucherIsExpiredError : VoucherIsExpiredErrorBase { }
    [Error("VoucherIsExpired")]
    public class VoucherIsExpiredErrorBase : IErrorDTO
    {
    }
}
