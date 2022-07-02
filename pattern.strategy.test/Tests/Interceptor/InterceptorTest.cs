using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using pattern.strategy.test.Fakes;
using pattern.strategy.test.Fakes.Interceptor;
using patterns.strategy;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using static pattern.strategy.test.Fakes.RequestFake;

namespace pattern.strategy.test.Tests.Validation
{
    public class InterceptorTest
    {
        [Fact]
        public void ValidateAsyncIsInvalidRequestTest()
        {
            var interceptor = new TestInterceptorAttribute();

            var invocationMock = new Mock<Castle.DynamicProxy.IInvocation>();

            invocationMock.SetupGet(o => o.MethodInvocationTarget)
                .Returns(typeof(RequestStrategyFake).GetMethod("HandleAsync"));

            invocationMock.SetupGet(o => o.Arguments)
                .Returns(new object[] { new Request() });

            interceptor.InterceptAsynchronous(invocationMock.Object);

            invocationMock.Verify(o => o.CaptureProceedInfo(), Times.Once);
        }

        [Fact]
        public void ValidateAsyncIsValidRequestTest()
        {
            var interceptor = new TestInterceptorAttribute();

            var invocationMock = new Mock<Castle.DynamicProxy.IInvocation>();

            var proceedInfo = new Mock<IInvocationProceedInfo>();

            proceedInfo.Setup(o => o.Invoke());

            var request = new Request() { Name = "Test Name" };

            invocationMock.SetupGet(o => o.ReturnValue)
                .Returns(new Task(() => { }));

            invocationMock.SetupGet(o => o.MethodInvocationTarget)
                .Returns(typeof(RequestStrategyFake).GetMethod("HandleAsync"));

            invocationMock.SetupGet(o => o.Arguments)
                .Returns(new object[] { request });

            invocationMock.Setup(o => o.CaptureProceedInfo())
                .Returns(proceedInfo.Object);

            interceptor.InterceptAsynchronous(invocationMock.Object);

            invocationMock.Verify(o => o.CaptureProceedInfo(), Times.Once);
        }

        [Fact]
        public async Task ValidateSyncProxyInterceptor()
        {
            var serviceColletion = new ServiceCollection();

            serviceColletion.AddScopedProxyInterceptor<ITestClassMethodInterceptor, TestClassMethodInterceptor>();

            var serviceProvider = serviceColletion.BuildServiceProvider();

            var proxy = serviceProvider.GetRequiredService<ITestClassMethodInterceptor>();

            proxy.SyncInterceptorVoid();
            proxy.SyncInterceptorResult();
            //await proxy.AsyncInterceptorVoid();
        }

        [Fact]
        public void SyncException()
        {
            var serviceColletion = new ServiceCollection();

            serviceColletion.AddScopedProxyInterceptor<ITestClassMethodInterceptor, TestClassMethodInterceptor>();

            var serviceProvider = serviceColletion.BuildServiceProvider();

            var proxy = serviceProvider.GetRequiredService<ITestClassMethodInterceptor>();

            Assert.Throws<NotImplementedException>(() => proxy.SyncInterceptorVoidException());
            //await proxy.AsyncInterceptorVoid();
        }

        [Fact]
        public async Task AsyncException()
        {
            var serviceColletion = new ServiceCollection();

            serviceColletion.AddScopedProxyInterceptor<ITestClassMethodInterceptor, TestClassMethodInterceptor>();

            var serviceProvider = serviceColletion.BuildServiceProvider();

            var proxy = serviceProvider.GetRequiredService<ITestClassMethodInterceptor>();

            await Assert.ThrowsAsync<NotImplementedException>(async () => await proxy.AsyncInterceptorVoidException());
        }
    }
}
