using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    /// <summary>
    /// Represents a query executed from a client that lies on top of a regular <see cref="IQuery{T}">Query</see>.
    /// </summary>    
    /// <typeparam name="TResult">Type of the result of the associated query.</typeparam>
    public abstract class ClientQuery<TResult> : ClientQuery<object, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientQuery{T}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected ClientQuery(IQuery<TResult> request)
            : this(request, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientQuery{T}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated query.</param>
        /// <param name="isValidIndicator">
        /// The indicator used to indicate whether or not the associated <paramref name="request"/> is valid (optional).
        /// </param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected ClientQuery(IQuery<TResult> request, IIsValidIndicator isValidIndicator)
            : this(request, isValidIndicator, ClientRequestExecutionOptions.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientQuery{T}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated query.</param>
        /// <param name="isValidIndicator">
        /// The indicator used to indicate whether or not the associated <paramref name="request"/> is valid (optional).
        /// </param>
        /// <param name="options">
        /// The execution options that determine the behavior of this request (optional).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected ClientQuery(IQuery<TResult> request, IIsValidIndicator isValidIndicator, ClientRequestExecutionOptions options)
            : base(request, isValidIndicator, options) { }

        /// <summary>
        /// Simply assigns <paramref name="parameterIn"/> to <paramref name="parameterOut"/> and returns <c>true</c>.
        /// </summary>
        /// <param name="parameterIn">The incoming parameter.</param>
        /// <param name="parameterOut">
        /// When this method has executed, refers to the same object as <paramref name="parameterIn"/>.
        /// </param>
        /// <returns><c>true</c></returns>
        protected override bool TryConvertParameter(object parameterIn, out object parameterOut)
        {
            parameterOut = parameterIn;
            return true;
        }
    }
}
