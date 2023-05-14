using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Truffle.Data
{
    public partial class UriEventDTO : UriEventDTOBase { }

    [Event("URI")]
    public class UriEventDTOBase : IEventDTO
    {
        [Parameter("string", "value", 1, false )]
        public virtual string Value { get; set; }
        [Parameter("uint256", "id", 2, true )]
        public virtual BigInteger Id { get; set; }
    }
}
