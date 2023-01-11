using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace pattern.strategy.test.Fakes.Interceptor
{
    public class TestInterceptorAttribute : InterceptorAttribute
    {
        protected override async Task<TResult> HandleInterceptAsync<TResult>(IInvocation invocation, Func<Task<TResult>> result)
        {
            var response = await result();
            return response;
        }
    }

    public class TestInterceptor2Attribute : InterceptorAttribute
    {
        protected override Task<TResult> HandleInterceptAsync<TResult>(IInvocation invocation, Func<Task<TResult>> result)
        {
            return result();
        }
    }
}
