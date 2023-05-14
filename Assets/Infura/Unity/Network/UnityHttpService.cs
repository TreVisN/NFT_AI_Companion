using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Infura.SDK;
using Infura.SDK.Network;
using Infura.Unity.Utils;
using UnityEngine.Networking;

namespace Infura.Unity.Network
{
    /// <summary>
    /// A Singleton class that handles all the network requests. It uses UnityWebRequest to make the requests.
    /// </summary>
    public class UnityHttpService : Singleton<UnityHttpService>
    {
        public enum RequestType
        {
            GET,
            POST,
            DELETE
        }
        
        /// <summary>
        /// A class that represents a single HTTP Request
        /// </summary>
        public class UnityHttpServiceRequest
        {
            public TaskCompletionSource<string> requestTask;
            public string url;
            public string @params;
            public RequestType requestType;
            public string authKey;
            public string authValue;
        }

        /// <summary>
        /// A class that implements the IHttpService interface.
        /// </summary>
        public class UnityHttpServiceProvider : IHttpService
        {
            private UnityHttpService service;
            private string baseUrl;
            private string authValue;
            private string authKey;

            public UnityHttpServiceProvider(string baseUrl, string authKey, string authValue, UnityHttpService service)
            {
                this.service = service;
                this.baseUrl = baseUrl;
                this.authValue = authValue;
                this.authKey = authKey;
            }

            public Task<string> Get(string uri)
            {
                var fullUrl = baseUrl.EndsWith("/") || uri.StartsWith("/") ? $"{baseUrl}{uri}" : $"{baseUrl}/{uri}";
                var request = new UnityHttpServiceRequest()
                {
                    url = fullUrl,
                    requestTask = new TaskCompletionSource<string>(),
                    authKey = authKey,
                    authValue = authValue,
                    requestType = RequestType.GET
                };
                
                service.requests.Enqueue(request);

                return request.requestTask.Task;
            }

            public Task<string> Post(string uri, string @params)
            {
                var fullUrl = baseUrl.EndsWith("/") || uri.StartsWith("/") ? $"{baseUrl}{uri}" : $"{baseUrl}/{uri}";
                var request = new UnityHttpServiceRequest()
                {
                    url = fullUrl,
                    requestTask = new TaskCompletionSource<string>(),
                    requestType = RequestType.POST,
                    @params = @params,
                    authKey = authKey,
                    authValue = authValue
                };
                
                service.requests.Enqueue(request);

                return request.requestTask.Task;
            }
            
            public Task<string> Delete(string uri, string @params)
            {
                var fullUrl = baseUrl.EndsWith("/") || uri.StartsWith("/") ? $"{baseUrl}{uri}" : $"{baseUrl}/{uri}";
                var request = new UnityHttpServiceRequest()
                {
                    url = fullUrl,
                    requestTask = new TaskCompletionSource<string>(),
                    requestType = RequestType.DELETE,
                    @params = @params,
                    authKey = authKey,
                    authValue = authValue
                };
                
                service.requests.Enqueue(request);

                return request.requestTask.Task;
            }
        }

        private Queue<UnityHttpServiceRequest> requests = new();
        private bool isCheckingQueue;

        private void Awake()
        {
            HttpServiceFactory.SetCreator(CreateHttpService);
        }

        private IHttpService CreateHttpService(string baseUrl, string authHeaderValue, string authHeaderName = "Authorization")
        {
            return new UnityHttpServiceProvider(baseUrl, authHeaderName, authHeaderValue, this);
        }

        private void Update()
        {
            if (!isCheckingQueue)
            {
                isCheckingQueue = true;
                StartCoroutine(ProcessQueue());
            }
        }

        private IEnumerator ProcessQueue()
        {
            while (requests.Count > 0)
            {
                yield return ProcessRequest(requests.Dequeue());
            }

            isCheckingQueue = false;
        }

        private IEnumerator ProcessRequest(UnityHttpServiceRequest request)
        {
            string method;
            switch (request.requestType)
            {
                case RequestType.POST:
                    method = "POST";
                    break;
                case RequestType.DELETE:
                    method = "DELETE";
                    break;
                case RequestType.GET:
                default:
                    method = "GET";
                    break;
            }
            
            using (UnityWebRequest uwr = request.requestType != RequestType.GET
                ? new UnityWebRequest(request.url, method)
                : UnityWebRequest.Get(request.url))
            {
                if (!string.IsNullOrWhiteSpace(request.authValue) && !string.IsNullOrWhiteSpace(request.authKey))
                {
                    uwr.SetRequestHeader(request.authKey, request.authValue);
                }

                uwr.SetRequestHeader("X-Infura-User-Agent", "infura/sdk-csharp 1.0.0");

                if (!string.IsNullOrWhiteSpace(request.@params))
                {
                    byte[] jsonToSend = Encoding.UTF8.GetBytes(request.@params);
                    uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
                    uwr.downloadHandler = new DownloadHandlerBuffer();
                    uwr.uploadHandler.contentType = "application/json";
                    uwr.SetRequestHeader("Content-Type", "application/json");
                }
                
                yield return uwr.SendWebRequest();

                switch (uwr.result)
                {
                    case UnityWebRequest.Result.ConnectionError: 
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        request.requestTask.SetException(new IOException(uwr.error + " | " + uwr.downloadHandler.text));
                        break;
                    case UnityWebRequest.Result.Success:
                        request.requestTask.SetResult(uwr.downloadHandler.text);
                        break;
                }
            }
        }
    }
}