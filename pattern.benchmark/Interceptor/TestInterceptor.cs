using AspectCore.DynamicProxy;
using Castle.DynamicProxy;
using pattern.proxy;
using System;
using System.Threading.Tasks;

namespace pattern.benchmark.Interceptor
{
    public class TestInterceptor : AsyncInterceptorBase
    {
        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            await proceed(invocation, proceedInfo);
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            return await proceed(invocation, proceedInfo);
        }
    }

    public class TestInterceptorAttribute : InterceptorAttribute
    {
        protected override async Task<TResult> HandleInterceptAsync<TResult>(IInvocation invocation, Func<Task<TResult>> result)
        {
            return await result.Invoke();
        }
    }

    public class TestInterceptorAspectoreAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            await next(context);
        }
    }
}
