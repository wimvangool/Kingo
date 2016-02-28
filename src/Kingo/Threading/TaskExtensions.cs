using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Threading
{
    /// <summary>
    /// Contains several extension methods for the <see cref="Task" /> class.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Awaits the specified task, blocking the current thread, and unwraps any <see cref="AggregateException" />
        /// by rethrowing the first inner exception, just like the await-statement would do.
        /// </summary>
        /// <param name="task">The task to await.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="task"/> is <c>null</c>.
        /// </exception>
        public static void Await(this Task task, CancellationToken? token = null)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            try
            {
                if (token.HasValue)
                {
                    task.Wait(token.Value);
                }
                else
                {
                    task.Wait();
                }                             
            }
            catch (AggregateException exception)
            {
                exception.RethrowInnerException();
            }
        }

        /// <summary>
        /// Awaits the specified task, blocking the current thread, and unwraps any <see cref="AggregateException" />
        /// by rethrowing the first inner exception, just like the await-statement would do.
        /// </summary>
        /// <param name="task">The task to await.</param>
        /// <param name="timeoutInMilliseconds">Maximum amount of time in milliseconds to wait for the task to complete.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns><c>true</c> if the Task completed execution within the allotted time; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="task"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeoutInMilliseconds"/> is not a valid timeout period.
        /// </exception>
        public static bool Await(this Task task, int timeoutInMilliseconds, CancellationToken? token = null)
        {
            return task.Await(TimeSpan.FromMilliseconds(timeoutInMilliseconds), token);
        }

        /// <summary>
        /// Awaits the specified task, blocking the current thread, and unwraps any <see cref="AggregateException" />
        /// by rethrowing the first inner exception, just like the await-statement would do.
        /// </summary>
        /// <param name="task">The task to await.</param>
        /// <param name="timeout">Maximum amount of time to wait for the task to complete.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns><c>true</c> if the Task completed execution within the allotted time; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="task"/> is <c>null</c>.
        /// </exception>
        public static bool Await(this Task task, TimeSpan timeout, CancellationToken? token = null)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            try
            {
                if (token.HasValue)
                {
                    return task.Wait(timeout.Milliseconds, token.Value);
                }
                return task.Wait(timeout.Milliseconds);
            }
            catch (AggregateException exception)
            {
                return exception.RethrowInnerException();
            }
        }

        /// <summary>
        /// Awaits the specified task, blocking the current thread, and unwraps any <see cref="AggregateException" />
        /// by rethrowing the first inner exception, just like the await-statement would do.
        /// </summary>
        /// <param name="task">The task to await.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="task"/> is <c>null</c>.
        /// </exception>
        public static TResult Await<TResult>(this Task<TResult> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Re-throws the first inner exception of the specified <paramref name="exception"/> if it has any
        /// inner exceptions, without losing the stacktrace of this exception.
        /// </summary>
        /// <param name="exception">The exception to unwrap.</param>
        /// <returns><c>false</c> if the exception was not re-thrown.</returns>
        /// <exception cref="Exception">
        /// If <paramref name="exception"/> has any inner exceptions.
        /// </exception>
        public static bool RethrowInnerException(this AggregateException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            if (exception.InnerExceptions.Count == 0)
            {
                return false;
            }
            exception.InnerExceptions[0].Rethrow();
            return true;
        }
    }
}
