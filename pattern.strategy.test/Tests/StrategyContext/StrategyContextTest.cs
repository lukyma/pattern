using Castle.DynamicProxy;
using FluentValidation.Results;
using Moq;
using pattern.strategy.test.Fakes;
using pattern.strategy.test.Fakes.Interceptor;
using patterns.strategy;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static pattern.strategy.test.Fakes.RequestFake;

namespace pattern.strategy.test.Tests.StrategyContext
{
    public class StrategyContextTest
    {
        [Fact]
        public async Task ValidateHandleFromStrategyContext()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(o => o.GetService(typeof(IStrategy<Request, Response>)))
                .Returns(new RequestStrategyFake());

            var strategyContext = new patterns.strategy.StrategyContext(serviceProviderMock.Object);

            var response = await strategyContext.HandlerAsync<Request, Response>(new Request(), CancellationToken.None);

            Assert.NotNull(response);
        }
    }
}
