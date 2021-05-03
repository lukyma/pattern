using System.Threading;
using System.Threading.Tasks;

namespace patterns.strategy
{
    public interface IStrategy<TRequest, TResponse> : IStrategy
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }

    public interface IStrategy
    {
    }
}
