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
    public partial class CreateForAdminMintFunction : CreateForAdminMintFunctionBase { }

    [Function("createForAdminMint")]
    public class CreateForAdminMintFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "tokenId_", 1)]
        public virtual BigInteger Tokenid { get; set; }
        [Parameter("uint256", "initialSupply_", 2)]
        public virtual BigInteger Initialsupply { get; set; }
        [Parameter("uint256", "maxSupply_", 3)]
        public virtual BigInteger Maxsupply { get; set; }
        [Parameter("string", "uri_", 4)]
        public virtual string Uri { get; set; }
    }
}
