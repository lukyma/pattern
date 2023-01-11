using Castle.DynamicProxy;
using patterns.strategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace pattern.strategy
{
    /// <summary>
    /// Attribute to be used in your interceptors.
    /// </summary>
    public abstract class InterceptorAttribute : AsyncInterceptorBaseAttribute
    {
        public int Order { get; set; }
        internal override Task InterceptAsync(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            return HandleInterceptAsync(invocation, async () => 
            { 
                await proceed(invocation, proceedInfo); 
                return Task.CompletedTask; 
            });
        }

        internal override Task<TResult> InterceptAsync<TResult>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            return HandleInterceptAsync(invocation, () => proceed(invocation, proceedInfo));
        }

        protected abstract Task<TResult> HandleInterceptAsync<TResult>(IInvocation invocation, Func<Task<TResult>> proceed);
    }

    public class InterceptorInfo
    {
        public int Order { get; set; }
        internal AsyncInterceptorBaseAttribute Interceptor { get; set; }
        internal string TypeClass { get; set; }
        internal string MethodName { get; set; }
    }

    /// <summary>
    /// Base attribute responsible for implementing the functions of the DynamicProxy interceptor. 
    /// Note: must not be used as an interceptor attribute. Please use InterceptorAttribute. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class AsyncInterceptorBaseAttribute : Attribute, IAsyncInterceptor
    {
        public AsyncInterceptorBaseAttribute()
        {
        }

        public List<InterceptorInfo> InterceptorInfos { get; set; } = new List<InterceptorInfo>();

        internal IServiceProvider ServiceProvider { get; set; }
        public string InterceptorAttributeName { get; set; }
        /// <summary>
        /// Returns the instance that is registered in the ServiceProvider.
        /// </summary>
        /// <typeparam name="T">Service register type</typeparam>
        /// <returns></returns>
        protected T GetService<T>()
        {
            return (T)ServiceProvider?.GetService(typeof(T));
        }
        protected object GetService(Type type)
        {
            return ServiceProvider?.GetService(type);
        }
        private static readonly MethodInfo InterceptSynchronousMethodInfo =
            typeof(AsyncInterceptorBaseAttribute).GetMethod(
                nameof(InterceptSynchronousResult), BindingFlags.Static | BindingFlags.NonPublic)!;

        private static readonly ConcurrentDictionary<Type, GenericSynchronousHandler> GenericSynchronousHandlers =
            new ConcurrentDictionary<Type, GenericSynchronousHandler>
            {
                [typeof(void)] = InterceptSynchronousVoid
            };

        private delegate void GenericSynchronousHandler(AsyncInterceptorBaseAttribute me, IInvocation invocation);

        /// <summary>
        /// Intercepts a synchronous method <paramref name="invocation"/>.
        /// </summary>
        /// <param name="invocation">The method invocation.</param>
        public void InterceptSynchronous(IInvocation invocation)
        {
            Type returnType = invocation.Method.ReturnType;
            GenericSynchronousHandler handler = GenericSynchronousHandlers.GetOrAdd(returnType, CreateHandler);
            handler(this, invocation);
        }

        /// <summary>
        /// Intercepts an asynchronous method <paramref name="invocation"/> with return type of <see cref="Task"/>.
        /// </summary>
        /// <param name="invocation">The method invocation.</param>
        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InterceptAsync(invocation, invocation.CaptureProceedInfo(), ProceedAsynchronous);
        }

        /// <summary>
        /// Intercepts an asynchronous method <paramref name="invocation"/> with return type of <see cref="Task{T}"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the <see cref="Task{T}"/> <see cref="Task{T}.Result"/>.</typeparam>
        /// <param name="invocation">The method invocation.</param>
        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue =
                InterceptAsync(invocation, invocation.CaptureProceedInfo(), ProceedAsynchronous<TResult>);
        }

        /// <summary>
        /// Override in derived classes to intercept method invocations.
        /// </summary>
        /// <param name="invocation">The method invocation.</param>
        /// <param name="proceedInfo">The <see cref="IInvocationProceedInfo"/>.</param>
        /// <param name="proceed">The function to proceed the <paramref name="proceedInfo"/>.</param>
        /// <returns>A <see cref="Task" /> object that represents the asynchronous operation.</returns>
        internal virtual Task InterceptAsync(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            if (invocation.MethodInvocationTarget
                .GetCustomAttributes(false).Any(p => p.GetType().IsSubclassOf(typeof(InterceptorAttribute))))
            {
                var parameters = string.Join('|', invocation.MethodInvocationTarget.GetParameters().Select(o => o.Name));

                var interceptorInfo = InterceptorInfos
                    .OrderBy(o => o.Order).FirstOrDefault(o => o.MethodName == parameters + invocation.MethodInvocationTarget.Name);
                if (interceptorInfo != null)
                {
                    return interceptorInfo.Interceptor
                        .InterceptAsync(invocation, proceedInfo, proceed);
                }
            }
            return proceed(invocation, proceedInfo);
        }

        /// <summary>
        /// Override in derived classes to intercept method invocations.
        /// </summary>
        /// <typeparam name="TResult">The type of the <see cref="Task{T}"/> <see cref="Task{T}.Result"/>.</typeparam>
        /// <param name="invocation">The method invocation.</param>
        /// <param name="proceedInfo">The <see cref="IInvocationProceedInfo"/>.</param>
        /// <param name="proceed">The function to proceed the <paramref name="proceedInfo"/>.</param>
        /// <returns>A <see cref="Task" /> object that represents the asynchronous operation.</returns>
        internal virtual Task<TResult> InterceptAsync<TResult>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            if (invocation.MethodInvocationTarget
                .GetCustomAttributes(false)
                .Any(p => p.GetType().IsSubclassOf(typeof(InterceptorAttribute))))
            {
                var parameters = string.Join('|', invocation.MethodInvocationTarget.GetParameters().Select(o => o.Name));

                var interceptorInfo = InterceptorInfos
                    .OrderBy(o => o.Order)
                    .FirstOrDefault(o => o.MethodName == parameters + invocation.MethodInvocationTarget.Name);
                if (interceptorInfo != null)
                {
                    return interceptorInfo.Interceptor.InterceptAsync(invocation, proceedInfo, proceed);
                }
            }
            return proceed(invocation, proceedInfo);
        }

        private static GenericSynchronousHandler CreateHandler(Type returnType)
        {
            MethodInfo method = InterceptSynchronousMethodInfo.MakeGenericMethod(returnType);
            return (GenericSynchronousHandler)method.CreateDelegate(typeof(GenericSynchronousHandler));
        }

        private static void InterceptSynchronousVoid(AsyncInterceptorBaseAttribute me, IInvocation invocation)
        {
            Task task = me.InterceptAsync(invocation, invocation.CaptureProceedInfo(), ProceedSynchronous);

            // If the intercept task has yet to complete, wait for it.
            if (!task.IsCompleted)
            {
                // Need to use Task.Run() to prevent deadlock in .NET Framework ASP.NET requests.
                // GetAwaiter().GetResult() prevents a thrown exception being wrapped in a AggregateException.
                // See https://stackoverflow.com/a/17284612
                Task.Run(() => task).GetAwaiter().GetResult();
            }

            task.RethrowIfFaulted();
        }

        private static void InterceptSynchronousResult<TResult>(AsyncInterceptorBaseAttribute me, IInvocation invocation)
        {
            Task<TResult> task = me.InterceptAsync(invocation, invocation.CaptureProceedInfo(), ProceedSynchronous<TResult>);

            // If the intercept task has yet to complete, wait for it.
            if (!task.IsCompleted)
            {
                // Need to use Task.Run() to prevent deadlock in .NET Framework ASP.NET requests.
                // GetAwaiter().GetResult() prevents a thrown exception being wrapped in a AggregateException.
                // See https://stackoverflow.com/a/17284612
                Task.Run(() => task).GetAwaiter().GetResult();
            }

            task.RethrowIfFaulted();
        }

        private static Task ProceedSynchronous(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            try
            {
                proceedInfo.Invoke();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        private static Task<TResult> ProceedSynchronous<TResult>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo)
        {
            try
            {
                proceedInfo.Invoke();
                return Task.FromResult((TResult)invocation.ReturnValue);
            }
            catch (Exception e)
            {
                return Task.FromException<TResult>(e);
            }
        }

        private static async Task ProceedAsynchronous(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            proceedInfo.Invoke();

            // Get the task to await.
            var originalReturnValue = (Task)invocation.ReturnValue;

            await originalReturnValue.ConfigureAwait(false);
        }

        private static async Task<TResult> ProceedAsynchronous<TResult>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo)
        {
            proceedInfo.Invoke();

            // Get the task to await.
            var originalReturnValue = (Task<TResult>)invocation.ReturnValue;

            TResult result = await originalReturnValue.ConfigureAwait(false);
            return result;
        }
    }
}
