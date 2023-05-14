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
    public partial class SignerRoleFunction : SignerRoleFunctionBase { }

    [Function("SIGNER_ROLE", "bytes32")]
    public class SignerRoleFunctionBase : FunctionMessage
    {

    }
}
