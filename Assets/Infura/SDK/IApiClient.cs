using System.Threading.Tasks;
using Infura.SDK.Network;

namespace Infura.SDK
{
    public interface IApiClient
    {
        IHttpService HttpClient { get; }

        Task ProcessItem<T>(T item);
    }
}