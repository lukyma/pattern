using pattern.benchmark.Interceptor;
using patterns.strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pattern.benchmark.Strategy
{
    public class Request
    {
        public string Name { get; set; }
    }
    public class TestStrategy : IStrategy<Request, Response>
    {
        [TestInterceptor]
        public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            return new Response();
        }
    }

    public class Response
    {
    }
}
