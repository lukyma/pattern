using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace patterns.strategy
{
    public static class RethrowHelper
    {
        public static void Rethrow(this Exception? exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            ExceptionDispatchInfo.Capture(exception).Throw();
        }
        public static void RethrowInnerIfAggregate(this Exception? exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            switch (exception)
            {
                case AggregateException aggregate:
                    Rethrow(aggregate.InnerException);
                    break;
                default:
                    Rethrow(exception);
                    break;
            }
        }
        public static void RethrowIfFaulted(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (task.IsFaulted)
                RethrowInnerIfAggregate(task.Exception);
        }
    }
}
