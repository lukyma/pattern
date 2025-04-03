using pattern.proxy.test.Fakes.Interceptor;
using pattern.proxy;
using System.Threading;
using System.Threading.Tasks;
using static pattern.proxy.test.Fakes.RequestFake;

namespace pattern.proxy.test.Fakes
{
    public class RequestStrategyFake : IProxy<Request, Response>
    {
        [TestInterceptor]
        public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new Response());
        }
    }

    public class Response
    {
    }
}
