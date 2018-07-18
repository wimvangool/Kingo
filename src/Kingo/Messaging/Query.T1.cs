using System;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a query as part of the <see cref="MicroProcessor" />'s pipeline.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the response that is returned by this query.</typeparam>
    public abstract class Query<TMessageOut> : MessageHandlerOrQuery<TMessageOut>
    {
        /// <summary>
        /// Creates and returns a result that can be returned by the pipeline without invoking the actual query.
        /// This method is typically used to return cached values.
        /// </summary>        
        /// <param name="result">Message that represents the cached result.</param>
        /// <returns>A result that can be returned from the query pipeline.</returns> 
        /// <exception cref="ArgumentException">
        /// <paramref name="result"/> could not be cast to <typeparamref name="TMessageOut"/>.
        /// </exception>
        public InvokeAsyncResult<TMessageOut> Yield(object result)
        {
            try
            {
                return Yield((TMessageOut) result);
            }
            catch (NullReferenceException)
            {
                throw NewInvalidResultException(result);
            }
            catch (InvalidCastException)
            {
                throw NewInvalidResultException(result);
            }
        }

        /// <summary>
        /// Creates and returns a result that can be returned by the pipeline without invoking the actual query.
        /// This method is typically used to return cached values.
        /// </summary>        
        /// <param name="result">Message that represents the cached result.</param>
        /// <returns>A result that can be returned from the query pipeline.</returns>        
        public InvokeAsyncResult<TMessageOut> Yield(TMessageOut result = default(TMessageOut)) =>
            new ExecuteAsyncResult<TMessageOut>(result);

        private static Exception NewInvalidResultException(object result)
        {
            var messageFormat = ExceptionMessages.Query_InvalidResult;
            var message = string.Format(messageFormat, result, typeof(TMessageOut).FriendlyName());
            return new ArgumentException(message, nameof(result));
        }
    }
}
