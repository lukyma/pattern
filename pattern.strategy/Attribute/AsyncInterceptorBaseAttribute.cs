using Castle.DynamicProxy;
using patterns.strategy;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace pattern.strategy
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public abstract class AsyncInterceptorBaseAttribute : Attribute, IAsyncInterceptor
    {
        public int Order { get; set; }
        internal string TypeClass { get; set; }
        private static readonly MethodInfo InterceptSynchronousMethodInfo =
            typeof(AsyncInterceptorBase).GetMethod(
                nameof(InterceptSynchronousResult), BindingFlags.Static | BindingFlags.NonPublic)!;

        private static readonly ConcurrentDictionary<Type, GenericSynchronousHandler> GenericSynchronousHandlers =
            new ConcurrentDictionary<Type, GenericSynchronousHandler>
            {
                [typeof(void)] = InterceptSynchronousVoid,
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
        protected abstract Task InterceptAsync(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task> proceed);

        /// <summary>
        /// Override in derived classes to intercept method invocations.
        /// </summary>
        /// <typeparam name="TResult">The type of the <see cref="Task{T}"/> <see cref="Task{T}.Result"/>.</typeparam>
        /// <param name="invocation">The method invocation.</param>
        /// <param name="proceedInfo">The <see cref="IInvocationProceedInfo"/>.</param>
        /// <param name="proceed">The function to proceed the <paramref name="proceedInfo"/>.</param>
        /// <returns>A <see cref="Task" /> object that represents the asynchronous operation.</returns>
        protected abstract Task<TResult> InterceptAsync<TResult>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed);

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
