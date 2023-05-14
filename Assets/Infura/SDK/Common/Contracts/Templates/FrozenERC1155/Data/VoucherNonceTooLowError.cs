using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Truffle.Data
{
    public partial class VoucherNonceTooLowError : VoucherNonceTooLowErrorBase { }
    [Error("VoucherNonceTooLow")]
    public class VoucherNonceTooLowErrorBase : IErrorDTO
    {
    }
}
