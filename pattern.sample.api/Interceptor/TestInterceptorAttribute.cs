using Castle.DynamicProxy;
using FluentValidation;
using pattern.sample.api.Service;
using pattern.sample.api.StrategyHandler.Validator;
using pattern.strategy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace pattern.sample.api.Interceptor
{
    public class TestInterceptorAttribute : InterceptorAttribute, IAsyncInterceptor
    {
        public string TypeTest { get; set; }

        protected override async Task<TResult> HandleInterceptAsync<TResult>(IInvocation invocation, Func<Task<TResult>> result)
        {
            var teste = GetService<ITestService>();
            var response = await result();
            return response;
        }
    }

    public class TestInterceptor2Attribute : InterceptorAttribute, IAsyncInterceptor
    {
        public TestInterceptor2Attribute()
        {
        }

        protected override Task<TResult> HandleInterceptAsync<TResult>(IInvocation invocation, Func<Task<TResult>> result)
        {
            return result.Invoke();
        }
    }

    public class ValidatorInterceptorAttribute : InterceptorAttribute, IAsyncInterceptor
    {
        private Type ValidationType { get; }
        public ValidatorInterceptorAttribute(Type validationType)
        {
            ValidationType = validationType;
        }

        private async Task<bool> ValidateAsync(IInvocation invocation)
        {
            IValidationErrors validationFailures = GetService<IValidationErrors>();
            var validatorInstance = (IValidator)Activator.CreateInstance(ValidationType);
            var validationContext = new ValidationContext<object>(invocation.Arguments.First());
            var validation = await validatorInstance.ValidateAsync(validationContext);
            foreach (var error in validation.Errors)
            {
                validationFailures.Add(error);
            }
            return validation.IsValid;
        }

        protected override async Task<TResult> HandleInterceptAsync<TResult>(IInvocation invocation, Func<Task<TResult>> result)
        {
            if (!await ValidateAsync(invocation))
            {
                return default;
            }

            return await result.Invoke();
        }
    }
}
