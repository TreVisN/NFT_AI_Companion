using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Truffle.Data
{
    public partial class VoucherRedeemedEventDTO : VoucherRedeemedEventDTOBase { }

    [Event("VoucherRedeemed")]
    public class VoucherRedeemedEventDTOBase : IEventDTO
    {
        [Parameter("bytes", "signature", 1, false )]
        public virtual byte[] Signature { get; set; }
    }
}
