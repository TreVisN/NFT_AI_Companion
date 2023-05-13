using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Infura.SDK.Common;
using Infura.SDK.Models;

namespace Infura.SDK
{
    /// <summary>
    /// A class that represents a prepared API call that can either be converted to an Observable, a Paginator
    /// or a list (fetched async). This API call requests a type TR, and returns a response T
    /// </summary>
    /// <typeparam name="TR">The API call request type</typeparam>
    /// <typeparam name="T">The API call response type</typeparam>
    public class PreparedApiCall<TR, T> : PreparedApiCall<TR, T, T> where TR : ICursor, IResponseSet<T>
    {
        public PreparedApiCall(string apiUrl, IApiClient client) : base(apiUrl, client, arg => arg)
        {
        }

        protected override IPaginator<T> Paginate(string apiUrl) => new Paginator<TR, T>(apiUrl, Client.HttpClient);
    }

    /// <summary>
    /// A class that represents a prepared API call that can either be converted to an Observable, a Paginator
    /// or a list (fetched async). This API call requests a type TR, returns a response TI and transforms the response
    /// to type T
    /// </summary>
    /// <typeparam name="TR">The API call request type</typeparam>
    /// <typeparam name="TI">The API call response type</typeparam>
    /// <typeparam name="T">The type the response type is transformed to</typeparam>
    public class PreparedApiCall<TR, TI, T> where TR : ICursor, IResponseSet<TI>
    {
        /// <summary>
        /// The URL for this API call
        /// </summary>
        public string ApiUrl { get; }
        
        /// <summary>
        /// The API client making this API call
        /// </summary>
        public IApiClient Client { get; }

        private Func<TI, T> Transform { get; }

        /// <summary>
        /// Create a new prepared API call
        /// </summary>
        /// <param name="apiUrl">The URL of the API call to make</param>
        /// <param name="client">The API client to use to make the API call</param>
        /// <param name="transform">A transformer function to transform the API response type to the type T</param>
        public PreparedApiCall(string apiUrl, IApiClient client, Func<TI, T> transform)
        {
            ApiUrl = apiUrl;
            Client = client;
            Transform = transform;
        }

        /// <summary>
        /// Get all elements in this API Route. This returns an
        /// observable that emits each element as it is obtained from the API. This is useful for
        /// "lazy-loading" elements, as the function will complete as soon as the Observable is created and started.
        /// For a "blocking" version of this function, use <see cref="AsListAsync"/>
        /// </summary>
        /// <returns>An observable that emits elements from the specified API route.
        /// This observable completes when all elements have been emitted.</returns>
        public IObservable<T> AsObservable() => ObservablePaginate(ApiUrl);

        /// <summary>
        /// Return all elements from this API route in a list. This task completes when the full list of elements
        /// are available, so it may take a while if the API route yields a large number of elements. For a "lazy-loading"
        /// version of this function, use <see cref="AsObservable"/> and <see cref="Paginate"/>
        /// </summary>
        /// <returns>A task that returns the full list of elements from the specified API Route.</returns>
        public Task<List<T>> AsListAsync() => ObservablePaginate(ApiUrl).AsListAsync();

        /// <summary>
        /// Get a <see cref="Paginator{R,T}"/> that can manually paginate the API call and return elements from
        /// each individual page. This can be useful for slowly reading data from the API
        /// </summary>
        /// <returns>A <see cref="Paginator{R,T}"/> that can paginate this API call</returns>
        public IPaginator<T> Paginate() => Paginate(ApiUrl);

        protected virtual IPaginator<T> Paginate(string apiUrl)
        {
            return new Paginator<TR, TI, T>(apiUrl, Client.HttpClient, Transform);
        }

        protected virtual IObservable<T> ObservablePaginate(string apiUrl)
        {
            var paginator = Paginate(apiUrl);

            return Observable.Create<T>(async (observer, cancel) =>
            {
                IPage<T> page = null;
                do
                {
                    try
                    {
                        page = await paginator.NextPage();

                        if (page == null) continue;

                        foreach (var item in page.Contents)
                        {
#pragma warning disable CS4014
                            Client.ProcessItem(item);
#pragma warning restore CS4014

                            observer.OnNext(item);
                        }
                    }
                    catch (Exception e)
                    {
                        observer.OnError(e);
                        return;
                    }
                } while (!cancel.IsCancellationRequested && page != null);
                
                observer.OnCompleted();
            });
        }
    }
}