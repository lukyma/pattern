﻿using pattern.sample.api.Interceptor;
using pattern.sample.api.Service;
using pattern.sample.api.StrategyHandler.Validator;
using patterns.strategy;
using System.Threading;
using System.Threading.Tasks;

namespace pattern.sample.api.StrategyHandler
{
    public class TestStrategy : IStrategy<TestStrategyRequest, TestStrategyResponse>
    {
        private ITestService TestService { get; }
        public TestStrategy(ITestService testService)
        {
            TestService = testService;
        }
        [ValidatorInterceptor(typeof(TestStrategyRequestValidator), Order = 1)]
        //[TestInterceptor(Order = 2)]
        public async Task<TestStrategyResponse> HandleAsync(TestStrategyRequest request, CancellationToken cancellationToken)
        {
            TestService.Teste1();
            //TestService.Teste2();
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
        public string Teste { get; set; } = "Teste";
    }

    public class TestStrategyRequest2
    {
        public string Name { get; set; }
    }

    public class TestStrategyResponse2
    {
    }
}
