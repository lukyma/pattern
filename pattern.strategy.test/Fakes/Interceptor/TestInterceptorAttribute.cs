using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace pattern.strategy.test.Fakes.Interceptor
{
    public class TestInterceptorAttribute : AsyncInterceptorBaseAttribute, IAsyncInterceptor
    {
        protected override Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            return proceed(invocation, proceedInfo);
        }

        protected override Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            return proceed(invocation, proceedInfo);
        }
    }
}
