using System;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Threading
{
    /// <summary>
    /// Contains several helper methods for async methods that need to run synchronously.
    /// </summary>
    public static class AsyncMethod
    {
        #region [====== RunSynchronously ======]

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
                throw new ArgumentNullException("action");
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
                throw new ArgumentNullException("func");
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
        public static Task<TResult> Value<TResult>(TResult returnValue)
        {
            return Task.FromResult(returnValue);
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
        public static Task Throw(Exception exception)
        {
            return Throw<object>(exception);
        }

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
                throw new ArgumentNullException("exception");
            }
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            taskCompletionSource.SetException(exception);
            return taskCompletionSource.Task;
        }

        #endregion                   
    }
}
