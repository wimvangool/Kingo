using System;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    /// <summary>
    /// Represents a command sent from a client that lies on top of a regular <see cref="ICommand">Command</see>.
    /// </summary>
    /// <typeparam name="TParameter">Type of the parameter that can be specified for executing this request.</typeparam>
    public abstract class ClientCommand<TParameter> : ClientRequest<ICommand, TParameter>        
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand{T}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected ClientCommand(ICommand request)
            : this(request, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand{T}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated command.</param>
        /// <param name="isValidIndicator">
        /// The indicator used to indicate whether or not the associated <paramref name="request"/> is valid (optional).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected ClientCommand(ICommand request, IIsValidIndicator isValidIndicator)
            : this(request, isValidIndicator, ClientRequestExecutionOptions.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand{T}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated command.</param>
        /// <param name="isValidIndicator">
        /// The indicator used to indicate whether or not the associated <paramref name="request"/> is valid (optional).
        /// </param>
        /// <param name="options">
        /// The execution options that determine the behavior of this request (optional).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected ClientCommand(ICommand request, IIsValidIndicator isValidIndicator, ClientRequestExecutionOptions options)
            : base(request, isValidIndicator, options) { }        
    }
}
