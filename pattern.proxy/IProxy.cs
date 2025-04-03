using System.Threading;
using System.Threading.Tasks;

namespace pattern.proxy
{
    public interface IProxy<TRequest, TResponse> : IProxy
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }

    public interface IProxy
    {
    }
}
