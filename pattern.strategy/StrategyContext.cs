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

        /// <summary>
        /// Responsible for making the interface between the main object.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TResponse> HandlerAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        {
            var strategy = ServiceProvider.GetService(typeof(IStrategy<TRequest, TResponse>)) as IStrategy<TRequest, TResponse>;
            try
            {
                return strategy.HandleAsync(request, cancellationToken);
            }
            catch
            {
                return strategy.HandleAsync(request, cancellationToken);
            }
            finally
            {
            }
        }
    }
}
