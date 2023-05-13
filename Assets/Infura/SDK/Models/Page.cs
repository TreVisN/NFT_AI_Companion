using System;
using System.Linq;
using Newtonsoft.Json;

namespace Infura.SDK.Models
{
    public class Page<R, TI, T> : IPage<T> where R : ICursor, IResponseSet<TI>
    {
        private Func<TI, T> _transformer;

        public int PageNumber { get; }
        
        [JsonProperty("cursor")]
        public string Cursor
        {
            get
            {
                return PageData != null ? PageData.Cursor : null;
            }
        }
        
        public R PageData { get; }

        public T[] Contents
        {
            get
            {
                return PageData.Data.Select(t => _transformer(t)).ToArray();
            }
        }

        public Page(R data, int page, Func<TI, T> transformer)
        {
            PageData = data;
            PageNumber = page;
            _transformer = transformer;
        }
    }
}