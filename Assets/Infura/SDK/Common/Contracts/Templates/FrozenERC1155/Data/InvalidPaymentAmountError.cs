using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Truffle.Data
{
    public partial class InvalidPaymentAmountError : InvalidPaymentAmountErrorBase { }

    [Error("InvalidPaymentAmount")]
    public class InvalidPaymentAmountErrorBase : IErrorDTO
    {
        [Parameter("string", "_message", 1)]
        public virtual string Message { get; set; }
    }
}
