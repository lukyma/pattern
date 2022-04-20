using Castle.DynamicProxy;
using FluentValidation;
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
        public TestInterceptorAttribute()
        {
        }

        protected override async Task<TResult> HandleInterceptAsync<TResult>(IInvocation invocation, Func<Task<TResult>> result)
        {
            var teste = await result.Invoke();
            return teste;
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
