using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Infura.SDK.Network;
using Newtonsoft.Json;

namespace Infura.SDK.Models
{
    public class Paginator<R, T> : Paginator<R, T, T> where R : ICursor, IResponseSet<T>
    {
        public Paginator(string apiUrl, IHttpService client) : base(apiUrl, client, arg => arg)
        {
        }
    }

    public class Paginator<R, TI, T> : IPaginator<T> where R : ICursor, IResponseSet<TI>
    {
        private Func<TI, T> _transformer;

        public string ApiUrl { get; }
        
        public IHttpService HttpClient { get; }

        public IPage<T> CurrentPage { get; private set; }
        
        public int PageNumber { get; private set; }

        private List<IPage<T>> _previousPages = new List<IPage<T>>();
        private List<IPage<T>> _futurePages = new List<IPage<T>>();

        public ReadOnlyCollection<IPage<T>> PreviousPages
        {
            get
            {
                return _previousPages.AsReadOnly();
            }
        }
        
        public Paginator(string apiUrl, IHttpService client, Func<TI, T> transformer)
        {
            ApiUrl = apiUrl;
            HttpClient = client;
            _transformer = transformer;
        }

        public async Task<IPage<T>> NextPage()
        {
            if (CurrentPage != null && CurrentPage.Cursor == null)
                return null; // No next page
            
            PageNumber++;
            if (_futurePages.Count > 0)
            {
                var futurePage = _futurePages.FirstOrDefault(p => p.PageNumber == PageNumber);
                if (futurePage != null)
                {
                    _futurePages.Remove(futurePage);
                    
                    if (CurrentPage != null)
                        _previousPages.Add(CurrentPage);
                    
                    CurrentPage = futurePage;
                    return futurePage;
                }
            }

            var cursor = CurrentPage != null ?  $"{(ApiUrl.Contains("?") ? "&" : "?")}cursor={CurrentPage.Cursor}" : "";
            var fullUrl = $"{ApiUrl}{cursor}";
            
            var json = await HttpClient.Get(fullUrl);
            var data = JsonConvert.DeserializeObject<R>(json);

            if (data == null) return null;

            var page = new Page<R, TI, T>(data, PageNumber, _transformer);
            
            if (CurrentPage != null)
                _previousPages.Add(CurrentPage);

            CurrentPage = page;

            return page;
        }

        public IPage<T> PreviousPage()
        {
            if (PageNumber > 1)
            {
                if (PageNumber - 1 >= PreviousPages.Count)
                    throw new IndexOutOfRangeException("Previous page not found");
                
                PageNumber--;
                
                if (CurrentPage != null)
                    _futurePages.Insert(0, CurrentPage);

                CurrentPage = _previousPages[PageNumber - 1];
                _previousPages.RemoveAll(p => p.PageNumber == PageNumber);
                
                return CurrentPage;
            }
            else
            {
                PageNumber--;
                if (PageNumber < 0)
                    PageNumber = 0;
                CurrentPage = null;
                return null;
            }
        }
    }
}