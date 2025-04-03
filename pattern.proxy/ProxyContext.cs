using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace pattern.proxy
{
    public class ProxyContext : IProxyContext
    {
        private IServiceProvider ServiceProvider { get; }
        public ProxyContext(IServiceProvider serviceProvider)
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
            var service = ServiceProvider.GetRequiredService(typeof(IProxy<TRequest, TResponse>)) as IProxy<TRequest, TResponse>;
            return service.HandleAsync(request, cancellationToken);
        }
    }
}
