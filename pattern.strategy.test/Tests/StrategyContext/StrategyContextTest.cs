using Castle.DynamicProxy;
using Moq;
using pattern.sample.api.Interceptor;
using pattern.sample.api.StrategyHandler.Validator;
using pattern.strategy.test.Fakes;
using patterns.strategy;
using System;
using System.Reflection;
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

        [Fact]
        public async Task ValidateHandleFromStrategyContextWithProxyValidatorInterceptor()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var strategyFake = new RequestStrategyFake();

            var proxyGenerator = new ProxyGenerator();

            serviceProviderMock.Setup(o => o.GetService(typeof(IValidationErrors)))
                .Returns(new ValidationErrors());

            var validatorInterceptor = new ValidatorInterceptorAttribute(typeof(RequestValidator));

            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IStrategy<Request, Response>),
                                                                      strategyFake,
                                                                      validatorInterceptor);

            serviceProviderMock.Setup(o => o.GetService(typeof(IStrategy<Request, Response>)))
                .Returns(proxy);

            validatorInterceptor
                .GetType()
                .GetProperty("ServiceProvider", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(validatorInterceptor, serviceProviderMock.Object);

            var strategyContext = new patterns.strategy.StrategyContext(serviceProviderMock.Object);

            var response = await strategyContext.HandlerAsync<Request, Response>(new Request(), CancellationToken.None);

            Assert.Null(response);
        }
    }
}
