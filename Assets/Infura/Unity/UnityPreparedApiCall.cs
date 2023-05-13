using System;
using System.Reactive.Linq;
using Infura.SDK;
using Infura.SDK.Models;

namespace Infura.Unity
{
    public class UnityPreparedApiCall<TR, T> : PreparedApiCall<TR, T> where TR : ICursor, IResponseSet<T>
    {
        private UnityMainThreadDispatcher _mtd;
        public UnityPreparedApiCall(string apiUrl, IApiClient client, UnityMainThreadDispatcher mtd) : base(apiUrl, client)
        {
            _mtd = mtd;
        }

        protected override IObservable<T> ObservablePaginate(string apiUrl)
        {
            return Observable.Create<T>(observer =>
            {
                var baseObservable = base.ObservablePaginate(apiUrl);
                return baseObservable.Subscribe(ts => _mtd.Enqueue(() => observer.OnNext(ts)), (e) => _mtd.Enqueue(() => observer.OnError(e)), () => _mtd.Enqueue(observer.OnCompleted));
            });
        }
    }
    
    public class UnityPreparedApiCall<TR, TI, T> : PreparedApiCall<TR, TI, T> where TR : ICursor, IResponseSet<TI>
    {
        private UnityMainThreadDispatcher _mtd;
        public UnityPreparedApiCall(string apiUrl, IApiClient client, Func<TI, T> transformer, UnityMainThreadDispatcher mtd) : base(apiUrl, client, transformer)
        {
            _mtd = mtd;
        }

        protected override IObservable<T> ObservablePaginate(string apiUrl)
        {
            return Observable.Create<T>(observer =>
            {
                var baseObservable = base.ObservablePaginate(apiUrl);
                return baseObservable.Subscribe(ts => _mtd.Enqueue(() => observer.OnNext(ts)), (e) => _mtd.Enqueue(() => observer.OnError(e)), () => _mtd.Enqueue(observer.OnCompleted));
            });
        }
    }
}