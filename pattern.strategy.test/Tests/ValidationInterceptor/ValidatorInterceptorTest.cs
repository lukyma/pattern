using Castle.DynamicProxy;
using Moq;
using pattern.strategy.test.Fakes;
using pattern.strategy.test.Fakes.Interceptor;
using System.Threading.Tasks;
using Xunit;
using static pattern.strategy.test.Fakes.RequestFake;

namespace pattern.strategy.test.Tests.Validation
{
    public class ValidatorInterceptorTest
    {
        [Fact]
        public void ValidateAsyncIsInvalidRequestTest()
        {
            var validatorInterceptor = new TestInterceptorAttribute();

            var invocationMock = new Mock<Castle.DynamicProxy.IInvocation>();

            invocationMock.SetupGet(o => o.MethodInvocationTarget)
                .Returns(typeof(RequestStrategyFake).GetMethod("HandleAsync"));

            invocationMock.SetupGet(o => o.Arguments)
                .Returns(new object[] { new Request() });

            validatorInterceptor.InterceptAsynchronous(invocationMock.Object);

            //Assert.True(validatorInterceptor.ValidationFailures.Any());
        }

        [Fact]
        public void ValidateAsyncIsValidRequestTest()
        {
            var validatorInterceptor = new TestInterceptorAttribute();

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

            validatorInterceptor.InterceptAsynchronous(invocationMock.Object);

            //Assert.False(validatorInterceptor.ValidationFailures.Any());
        }
    }
}
