using Castle.DynamicProxy;
using Moq;
using pattern.proxy.test.Fakes;
using pattern.proxy.test.Fakes.Interceptor;
using pattern.proxy;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static pattern.proxy.test.Fakes.RequestFake;

namespace pattern.proxy.test.Tests.StrategyContext
{
    public class StrategyContextTest
    {
        [Fact]
        public async Task ValidateHandleFromStrategyContext()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(o => o.GetService(typeof(IProxy<Request, Response>)))
                .Returns(new RequestStrategyFake());

            var strategyContext = new pattern.proxy.ProxyContext(serviceProviderMock.Object);

            var response = await strategyContext.HandlerAsync<Request, Response>(new Request(), CancellationToken.None);

            Assert.NotNull(response);
        }

        [Fact]
        public async Task ValidateHandleFromStrategyContext_ExceptionRequiredService()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(o => o.GetService(typeof(IProxy<Request, Response>)))
                .Returns(null);

            var strategyContext = new pattern.proxy.ProxyContext(serviceProviderMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await strategyContext.HandlerAsync<Request, Response>(new Request(), CancellationToken.None));
        }

        [Fact]
        public async Task HandlerAsyncFromStrategyContextWithProxyInterceptor()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var strategyFake = new RequestStrategyFake();

            var proxyGenerator = new ProxyGenerator();

            var validatorInterceptor = new TestInterceptorAttribute();

            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IProxy<Request, Response>),
                                                                      strategyFake,
                                                                      validatorInterceptor);

            serviceProviderMock.Setup(o => o.GetService(typeof(IProxy<Request, Response>)))
                .Returns(proxy);

            validatorInterceptor
                .GetType()
                .GetProperty("ServiceProvider", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(validatorInterceptor, serviceProviderMock.Object);

            var strategyContext = new pattern.proxy.ProxyContext(serviceProviderMock.Object);

            var response = await strategyContext.HandlerAsync<Request, Response>(new Request(), CancellationToken.None);

            Assert.IsType<Response>(response);
        }
    }
}
