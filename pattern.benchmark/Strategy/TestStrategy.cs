using pattern.benchmark.Interceptor;
using pattern.proxy;
using System.Threading;
using System.Threading.Tasks;

namespace pattern.benchmark.Strategy
{
    public class Request
    {
        public string Name { get; set; }
    }

    public class TestStrategy : IProxy<Request, Response>
    {
        [TestInterceptor]
        public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new Response());
        }
    }

    public interface IStrategyAspectCore : IProxy<Request, Response>
    {
        [TestInterceptorAspectore]
        Task<Response> HandleAsync(Request request, CancellationToken cancellationToken);
    }

    public class TestStrategyAspectCore : IStrategyAspectCore
    {
        public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new Response());
        }
    }

    public class Response
    {
    }
}
