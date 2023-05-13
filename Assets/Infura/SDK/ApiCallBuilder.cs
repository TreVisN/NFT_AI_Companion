using System;
using Infura.SDK.Models;

namespace Infura.SDK
{
    public class ApiCallBuilder
    {
        public virtual PreparedApiCall<TR, T> BuildApiCall<TR, T>(IApiClient client, string api) where TR : ICursor, IResponseSet<T>
        {
            return new PreparedApiCall<TR, T>(api, client);
        }
        
        public virtual PreparedApiCall<TR, TI, T> BuildApiCall<TR, TI, T>(IApiClient client, string api, Func<TI, T> transformer) where TR : ICursor, IResponseSet<TI>
        {
            return new PreparedApiCall<TR, TI, T>(api, client, transformer);
        }
    }
}