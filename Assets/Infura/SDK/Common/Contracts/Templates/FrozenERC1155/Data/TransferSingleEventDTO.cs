using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Truffle.Data
{
    public partial class TransferSingleEventDTO : TransferSingleEventDTOBase { }

    [Event("TransferSingle")]
    public class TransferSingleEventDTOBase : IEventDTO
    {
        [Parameter("address", "operator", 1, true )]
        public virtual string Operator { get; set; }
        [Parameter("address", "from", 2, true )]
        public virtual string From { get; set; }
        [Parameter("address", "to", 3, true )]
        public virtual string To { get; set; }
        [Parameter("uint256", "id", 4, false )]
        public virtual BigInteger Id { get; set; }
        [Parameter("uint256", "value", 5, false )]
        public virtual BigInteger Value { get; set; }
    }
}
