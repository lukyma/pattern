using Castle.DynamicProxy;
using FluentValidation;
using pattern.strategy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace patterns.strategy
{
    public class ValidatorInterceptor : AsyncInterceptorBase, IAsyncValidatorInterceptor
    {
        public IValidationErrors ValidationFailures { get; }
        public ValidatorInterceptor(IValidationErrors validationFailures = null)
        {
            ValidationFailures = validationFailures;
        }

        private async Task<bool> ValidateAsync(IInvocation invocation)
        {
            var validationAttribute = Attribute.GetCustomAttribute(invocation.MethodInvocationTarget, typeof(ValidatorAttribute));

            if (validationAttribute != null)
            {
                ValidatorAttribute attribute = (ValidatorAttribute)validationAttribute;
                var validatorInstance = (IValidator)Activator.CreateInstance(attribute.ValidationType);
                var validationContext = new ValidationContext<object>(invocation.Arguments.First());
                var validation = await validatorInstance.ValidateAsync(validationContext);
                foreach (var error in validation.Errors)
                {
                    ValidationFailures.Add(error);
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
