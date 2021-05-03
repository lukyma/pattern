using System;
using System.Threading;
using System.Threading.Tasks;

namespace patterns.strategy
{
    public class StrategyContext : IStrategyContext
    {
        private IServiceProvider ServiceProvider;
        public StrategyContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public Task<TResponse> HandlerAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        {
            var strategy = ServiceProvider.GetService(typeof(IStrategy<TRequest, TResponse>)) as IStrategy<TRequest, TResponse>;
            return strategy.HandleAsync(request, cancellationToken);
        }
    }
}
