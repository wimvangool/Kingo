using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Threading
{
    /// <summary>
    /// Contains several helper methods for async methods that need to run synchronously.
    /// </summary>
    public static class AsyncMethod
    {
        #region [====== RunSynchronously ======]

        /// <summary>
        /// Executes the specified asynchronous <paramref name="asyncFunc"/> synchronously and
        /// returns a completed <see cref="Task" /> while encapsulation any exceptions
        /// that might be thrown.
        /// </summary>        
        /// <param name="asyncFunc">The delegate to invoke.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asyncFunc"/> is <c>null</c>.
        /// </exception>
        public static Task RunSynchronously(Func<Task> asyncFunc)
        {
            if (asyncFunc == null)
            {
                throw new ArgumentNullException(nameof(asyncFunc));
            }
            return RunSynchronously(() => asyncFunc.Invoke().Await());
        }

        /// <summary>
        /// Executes the specified <paramref name="action"/> synchronously and
        /// returns a completed <see cref="Task" /> while encapsulation any exceptions
        /// that might be thrown.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        public static Task RunSynchronously(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            try
            {
                action.Invoke();                
            }
            catch (Exception exception)
            {
                return Throw(exception);
            }
            return Void;
        }

        /// <summary>
        /// Executes the specified asynchronous <paramref name="asyncFunc"/> synchronously and
        /// returns a completed <see cref="Task" /> while encapsulation any exceptions
        /// that might be thrown.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the delegate.</typeparam>        
        /// <param name="asyncFunc">The delegate to invoke.</param>
        /// <returns>A completed <see cref="Task" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asyncFunc"/> is <c>null</c>.
        /// </exception>
        public static Task<TResult> RunSynchronously<TResult>(Func<Task<TResult>> asyncFunc)
        {
            if (asyncFunc == null)
            {
                throw new ArgumentNullException(nameof(asyncFunc));
            }
            return RunSynchronously(() => asyncFunc.Invoke().Await());
        }

        /// <summary>
        /// Executes the specified <paramref name="func"/> synchronously and
        /// returns a completed <see cref="Task{T}" /> while encapsulation any exceptions
        /// that might be thrown.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the delegate.</typeparam>
        /// <param name="func">The delegate to invoke.</param>
        /// <returns>A completed <see cref="Task{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func"/> is <c>null</c>.
        /// </exception>
        public static Task<TResult> RunSynchronously<TResult>(Func<TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            try
            {
                return Value(func.Invoke());
            }
            catch (Exception exception)
            {
                return Throw<TResult>(exception);
            }           
        }

        #endregion

        #region [====== Return Values ======]

        /// <summary>
        /// Represents a completed <see cref="Task" /> that can be returned
        /// from a synchronous method with an asynchronous signature.
        /// </summary>
        public static readonly Task Void = Task.FromResult(new object());

        /// <summary>
        /// Creates and returns a completed <see cref="Task{T}" /> that can be returned
        /// from a synchronous method with an asynchronous signature.
        /// </summary>
        /// <typeparam name="TResult">Type of the result to return.</typeparam>
        /// <param name="returnValue">The result to return.</param>
        /// <returns>A completed <see cref="Task{T}" />.</returns>
        public static Task<TResult> Value<TResult>(TResult returnValue) =>
            Task.FromResult(returnValue);

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
