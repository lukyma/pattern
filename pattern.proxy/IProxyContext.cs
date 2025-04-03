using System.Threading;
using System.Threading.Tasks;

namespace pattern.proxy
{
    public interface IProxyContext
    {
        Task<TResponse> HandlerAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);
    }
}
