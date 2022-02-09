using pattern.strategy.test.Fakes.Interceptor;
using patterns.strategy;
using System.Threading;
using System.Threading.Tasks;
using static pattern.strategy.test.Fakes.RequestFake;

namespace pattern.strategy.test.Fakes
{
    public class RequestStrategyFake : IStrategy<Request, Response>
    {
        [TestInterceptor(Order = 1)]
        public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            return new Response();
        }
    }

    public class Response
    {
    }
}
