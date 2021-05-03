﻿using patterns.strategy;
using System.Threading;
using System.Threading.Tasks;
using static pattern.strategy.test.Fakes.RequestFake;

namespace pattern.strategy.test.Fakes
{
    public class RequestStrategyFake : IStrategy<Request, Response>
    {
        [Validator(typeof(RequestValidator))]
        public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            return new Response();
        }
    }

    public class Response
    {
    }
}
