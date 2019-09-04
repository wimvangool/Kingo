using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Threading
{
    /// <summary>
    /// Contains several helper methods for async methods.
    /// </summary>
    public static class AsyncMethod
    {
        #region [====== Run ======]

        /// <summary>
        /// Executes the specified asynchronous <paramref name="asyncFunc"/> synchronously and
        /// returns a completed <see cref="Task" /> while encapsulating any exceptions
        /// that might be thrown.
        /// </summary>        
        /// <param name="asyncFunc">The delegate to invoke.</param>
        /// <param name="token">
        /// If specified, this token is checked before and after <paramref name="asyncFunc"/> is executed,
        /// and a cancelled task is returned if cancellation has been requested.
        /// </param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asyncFunc"/> is <c>null</c>.
        /// </exception>
        public static Task Run(Func<Task> asyncFunc, CancellationToken? token = null)
        {
            if (asyncFunc == null)
            {
                throw new ArgumentNullException(nameof(asyncFunc));
            }
            return Run(() => asyncFunc.Invoke().Await(), token);
        }

        /// <summary>
        /// Executes the specified <paramref name="action"/> synchronously and
        /// returns a completed <see cref="Task" /> while encapsulating any exceptions
        /// that might be thrown.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        /// <param name="token">
        /// If specified, this token is checked before and after <paramref name="action"/> is executed,
        /// and a cancelled task is returned if cancellation has been requested.
        /// </param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        public static Task Run(Action action, CancellationToken? token = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }            
            if (TryGetCanceledTask(token, out var canceledTask))
            {
                return canceledTask;
            }
            try
            {
                action.Invoke();                
            }
            catch (OperationCanceledException exception)
            {
                if (TryGetCanceledTask(token, exception, out canceledTask))
                {
                    return canceledTask;
                }
                return Throw(exception);
            }
            catch (Exception exception)
            {
                return Throw(exception);
            }
            return Task.CompletedTask;
        }        

        /// <summary>
        /// Executes the specified <paramref name="func"/> synchronously and
        /// returns a completed <see cref="Task{T}" /> while encapsulation any exceptions
        /// that might be thrown.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the delegate.</typeparam>
        /// <param name="func">The delegate to invoke.</param>
        /// <param name="token">
        /// If specified, this token is checked before and after <paramref name="func"/> is executed,
        /// and a cancelled task is returned if cancellation has been requested.
        /// </param>
        /// <returns>A completed <see cref="Task{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func"/> is <c>null</c>.
        /// </exception>
        public static Task<TResult> Run<TResult>(Func<TResult> func, CancellationToken? token = null)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            if (TryGetCanceledTask(token, out Task<TResult> canceledTask))
            {
                return canceledTask;
            }
            try
            {
                return Task.FromResult(func.Invoke());
            }
            catch (OperationCanceledException exception)
            {
                if (TryGetCanceledTask(token, exception, out canceledTask))
                {
                    return canceledTask;
                }
                return Throw<TResult>(exception);
            }
            catch (Exception exception)
            {
                return Throw<TResult>(exception);
            }           
        }

        #endregion

        #region [====== Cancellation ======]


        public static Task OrAbort(this Task task, CancellationToken token, TimeSpan? timeout = null) =>
            Task.WhenAny(task, token.WaitForCancellation(timeout));

        /// <summary>
        /// Creates and returns a <see cref="Task"/> that will complete in the cancelled state as soon as the specified
        /// <paramref name="token"/> is signaled or as soon as the specified <paramref name="timeout"/> expires.
        /// </summary>
        /// <param name="token">
        /// The token that can be used to cancel and abort the operation immediately.
        /// </param>
        /// <param name="timeout">
        /// The maximum time to wait for the operation to be cancelled. If not specified, the timeout is infinite.
        /// </param>
        /// <returns>
        /// A task that will be completed as soon as <paramref name="token"/> is signaled or the specified
        /// <paramref name="timeout"/> expires.
        /// </returns>
        public static Task WaitForCancellation(this CancellationToken token, TimeSpan? timeout = null) =>
            token.WaitForCancellation(timeout ?? Timeout.InfiniteTimeSpan);

        private static Task WaitForCancellation(this CancellationToken token, TimeSpan timeout) =>
            Task.Delay(timeout, token);

        private static bool TryGetCanceledTask(CancellationToken? token, OperationCanceledException exception, out Task canceledTask)
        {
            if (token.HasValue && token.Value.IsCancellationRequested && token.Value.Equals(exception.CancellationToken))
            {
                canceledTask = Task.FromCanceled(token.Value);
                return true;
            }
            canceledTask = null;
            return false;
        }

        private static bool TryGetCanceledTask<TResult>(CancellationToken? token, OperationCanceledException exception, out Task<TResult> canceledTask)
        {
            if (token.HasValue && token.Value.IsCancellationRequested && token.Value.Equals(exception.CancellationToken))
            {
                canceledTask = Task.FromCanceled<TResult>(token.Value);
                return true;
            }
            canceledTask = null;
            return false;
        }

        private static bool TryGetCanceledTask(CancellationToken? token, out Task canceledTask)
        {
            if (token.HasValue && token.Value.IsCancellationRequested)
            {
                canceledTask = Task.FromCanceled(token.Value);
                return true;
            }
            canceledTask = null;
            return false;
        }

        private static bool TryGetCanceledTask<TResult>(CancellationToken? token, out Task<TResult> canceledTask)
        {
            if (token.HasValue && token.Value.IsCancellationRequested)
            {
                canceledTask = Task.FromCanceled<TResult>(token.Value);
                return true;
            }
            canceledTask = null;
            return false;
        }       

        #endregion

        #region [====== Throw Exception ======]

        /// <summary>
        /// Creates and returns a new completed <see cref="Task{T}" /> that encapsulates
        /// the specified <paramref name="exception"/>.
        /// </summary>        
        /// <param name="exception">The exception to encapsulate.</param>
        /// <returns>A new completed <see cref="Task{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        public static Task Throw(Exception exception) =>
            Throw<object>(exception);

        /// <summary>
        /// Creates and returns a new completed <see cref="Task{T}" /> that encapsulates
        /// the specified <paramref name="exception"/>.
        /// </summary>
        /// <typeparam name="TResult">Return-type of the <see cref="Task{T}" />.</typeparam>
        /// <param name="exception">The exception to encapsulate.</param>
        /// <returns>A new completed <see cref="Task{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        public static Task<TResult> Throw<TResult>(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            return Task.FromException<TResult>(exception);            
        }

        #endregion

        #region [====== Await ======]

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
                throw;
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
        public static bool Await(this Task task, int timeoutInMilliseconds, CancellationToken? token = null) =>
             task.Await(TimeSpan.FromMilliseconds(timeoutInMilliseconds), token);

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
                    return task.Wait((int) timeout.TotalMilliseconds, token.Value);
                }
                return task.Wait((int) timeout.TotalMilliseconds);
            }
            catch (AggregateException exception)
            {
                exception.RethrowInnerException();
                throw;
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
            try
            {
                return task.GetAwaiter().GetResult();
            }
            catch (AggregateException exception)
            {
                exception.RethrowInnerException();
                throw;
            }
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
        public static void RethrowInnerException(this AggregateException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            if (exception.InnerExceptions.Count > 0)
            {
                exception.Flatten().InnerExceptions[0].Rethrow();
            }
        }

        #endregion
    }
}
