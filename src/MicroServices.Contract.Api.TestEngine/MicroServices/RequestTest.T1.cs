using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a test-class for a specific <see cref="Request" />.
    /// </summary>    
    public abstract class RequestTest<TRequest> where TRequest : class
    {        
        #region [====== AssertThat ======]

        /// <summary>
        /// Creates and returns a wrapper for a request that is configured using the specified
        /// <paramref name="requestConfigurator" />.
        /// </summary>
        /// <param name="requestConfigurator">
        /// A delegate that is used to configure the properties of a request before its expected state is verified.
        /// </param>
        /// <returns>A new <see cref="IRequest"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="requestConfigurator"/> is <c>null</c>.
        /// </exception>
        protected IRequest AssertThat(Action<TRequest> requestConfigurator)
        {
            if (requestConfigurator == null)
            {
                throw new ArgumentNullException(nameof(requestConfigurator));
            }
            var request = CreateRequest();
            requestConfigurator.Invoke(request);
            return AssertThat(request);
        }

        /// <summary>
        /// Creates and returns a wrapper for the specified <paramref name="request" /> that can be used
        /// to assert whether or not the request is valid.
        /// </summary>
        /// <param name="request">The request to test.</param>
        /// <returns>A new <see cref="IRequest"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected virtual IRequest AssertThat(TRequest request) =>
            new Request(request);

        /// <summary>
        /// Creates and returns a new request.
        /// </summary>
        /// <returns>A new request message.</returns>
        protected abstract TRequest CreateRequest();        

        #endregion
    }
}
