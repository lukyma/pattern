﻿using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace pattern.sample.api.Helper
{
    internal static class RethrowHelper
    {
        /// <summary>
        /// Re-throws the supplied exception without losing its stack trace.
        /// Prefer <c>throw;</c> where possible, this method is useful for re-throwing
        /// <see cref="Exception.InnerException" /> which cannot be done with the <c>throw;</c> semantics.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void Rethrow(this Exception? exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        /// <summary>
        /// If the <paramref name="exception"/> is an <see cref="AggregateException"/> the
        /// <paramref name="exception"/>.<see cref="Exception.InnerException"/> is re-thrown; otherwise the
        /// <paramref name="exception"/> is re-thrown.
        /// </summary>
        /// <param name="exception">The exception.</param>
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

        /// <summary>
        /// If the <paramref name="task"/> <see cref="Task.IsFaulted"/> the inner exception is re-thrown; otherwise the
        /// method is a no-op.
        /// </summary>
        /// <param name="task">The task.</param>
        public static void RethrowIfFaulted(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (task.IsFaulted)
                RethrowInnerIfAggregate(task.Exception);
        }
    }
}
