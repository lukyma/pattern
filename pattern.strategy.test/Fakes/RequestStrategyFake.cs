﻿using pattern.strategy.test.Fakes.Interceptor;
using patterns.strategy;
using System.Threading;
using System.Threading.Tasks;
using static pattern.strategy.test.Fakes.RequestFake;

namespace pattern.strategy.test.Fakes
{
    public class RequestStrategyFake : IStrategy<Request, Response>
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
