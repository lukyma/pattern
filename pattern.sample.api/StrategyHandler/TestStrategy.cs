using pattern.sample.api.Interceptor;
using pattern.sample.api.StrategyHandler.Validator;
using patterns.strategy;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace pattern.sample.api.StrategyHandler
{
    public class TestStrategy : IStrategy<TestStrategyRequest, TestStrategyResponse>
    {
        [ValidatorInterceptor(typeof(TestStrategyRequestValidator), Order = 1)]
        public async Task<TestStrategyResponse> HandleAsync(TestStrategyRequest request, CancellationToken cancellationToken)
        {
            //var teste = this.GetType().GetProperty("Teste", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return await HandleAsync2(request, cancellationToken);
        }
        //[TestInterceptor(Order = 1)]
        public async Task<TestStrategyResponse> HandleAsync2(TestStrategyRequest request, CancellationToken cancellationToken)
        {
            return new TestStrategyResponse();
        }
    }
    public class TestStrategy2 : IStrategy<TestStrategyRequest2, TestStrategyResponse2>
    {
        [TestInterceptor(Order = 1)]
        public async Task<TestStrategyResponse2> HandleAsync(TestStrategyRequest2 request, CancellationToken cancellationToken)
        {
            return await HandleAsync2(request, cancellationToken);
        }
        public async Task<TestStrategyResponse2> HandleAsync2(TestStrategyRequest2 request, CancellationToken cancellationToken)
        {
            return new TestStrategyResponse2();
        }
    }
    public class TestStrategyRequest
    {
        public string Name { get; set; }
    }
    public class TestStrategyResponse
    {
    }

    public class TestStrategyRequest2
    {
        public string Name { get; set; }
    }

    public class TestStrategyResponse2
    {
    }
}
