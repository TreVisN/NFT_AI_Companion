using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Truffle.Data
{
    public partial class CannotBeZeroAddressError : CannotBeZeroAddressErrorBase { }

    [Error("CannotBeZeroAddress")]
    public class CannotBeZeroAddressErrorBase : IErrorDTO
    {
        [Parameter("string", "_message", 1)]
        public virtual string Message { get; set; }
    }
}
