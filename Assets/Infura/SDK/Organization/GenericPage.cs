using Infura.SDK.Models;
using Newtonsoft.Json;

namespace Infura.SDK.Organization
{
    public class GenericPage<T> : ICursor, IResponseSet<T>
    {
        
        [JsonProperty("cursor")]
        public string Cursor { get; set; }
        
        [JsonProperty("results")]
        public T[] Data { get; set; }
        
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
        
        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
    }
}