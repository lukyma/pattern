using System.Threading;
using System.Threading.Tasks;

namespace patterns.strategy
{
    public interface IStrategyContext
    {
        Task<TResponse> HandlerAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);
    }
}
