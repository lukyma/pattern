using Castle.DynamicProxy;
using FluentValidation;
using pattern.sample.api.StrategyHandler;
using pattern.sample.api.StrategyHandler.Validator;
using pattern.strategy;
using patterns.strategy;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace pattern.sample.api.Interceptor
{
    public class TestInterceptorAttribute : AsyncInterceptorBaseAttribute, IAsyncInterceptor
    {
        public TestInterceptorAttribute()
        {
        }
        protected override Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            return proceed(invocation, proceedInfo);
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            return await proceed(invocation, proceedInfo);
        }
    }

    public class ValidatorInterceptorAttribute : AsyncInterceptorBaseAttribute, IAsyncInterceptor
    {
        private Type ValidationType { get; }
        public ValidatorInterceptorAttribute(Type validationType)
        {
            ValidationType = validationType;
        }

        private async Task<bool> ValidateAsync(IInvocation invocation)
        {
            IValidationErrors validationFailures = GetService<IValidationErrors>();

            var validationAttribute = GetCustomAttribute(invocation.MethodInvocationTarget, typeof(ValidatorInterceptorAttribute));

            if (validationAttribute != null)
            {
                var validatorInstance = (IValidator)Activator.CreateInstance(ValidationType);
                var validationContext = new ValidationContext<object>(invocation.Arguments.First());
                var validation = await validatorInstance.ValidateAsync(validationContext);
                foreach (var error in validation.Errors)
                {
                    validationFailures.Add(error);
                }
                return validation.IsValid;
            }

            return true;
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            if (!await ValidateAsync(invocation))
            {
                return;
            }

            await proceed(invocation, proceedInfo).ConfigureAwait(false);
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            if (!await ValidateAsync(invocation))
            {
                return default;
            }

            return await proceed(invocation, proceedInfo).ConfigureAwait(false);
        }
    }
}
