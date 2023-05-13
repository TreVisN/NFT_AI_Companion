using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Infura.SDK.Network;

namespace Infura.SDK.Models
{
    public interface IPaginator<T>
    {
        string ApiUrl { get; }
        
        IHttpService HttpClient { get; }
        
        IPage<T> CurrentPage { get; }
        
        int PageNumber { get; }
        
        ReadOnlyCollection<IPage<T>> PreviousPages { get; }

        Task<IPage<T>> NextPage();

        IPage<T> PreviousPage();
    }
}