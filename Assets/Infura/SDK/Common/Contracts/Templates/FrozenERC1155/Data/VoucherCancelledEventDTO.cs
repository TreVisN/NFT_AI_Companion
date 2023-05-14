using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Truffle.Data
{
    public partial class VoucherCancelledEventDTO : VoucherCancelledEventDTOBase { }

    [Event("VoucherCancelled")]
    public class VoucherCancelledEventDTOBase : IEventDTO
    {
        [Parameter("address", "minter", 1, false )]
        public virtual string Minter { get; set; }
        [Parameter("uint256", "nonce", 2, false )]
        public virtual BigInteger Nonce { get; set; }
    }
}
