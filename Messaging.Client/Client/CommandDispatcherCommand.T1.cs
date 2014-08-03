namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a command sent from a client that lies on top of a regular <see cref="ICommandDispatcher">Command</see>.
    /// </summary>
    /// <typeparam name="TParameter">Type of the parameter that can be specified for executing this request.</typeparam>
    public abstract class CommandDispatcherCommand<TParameter> : RequestDispatcherCommand<ICommandDispatcher, TParameter>        
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDispatcherCommand{T}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected CommandDispatcherCommand(ICommandDispatcher request)
            : this(request, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDispatcherCommand{T}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated command.</param>
        /// <param name="isValidIndicator">
        /// The indicator used to indicate whether or not the associated <paramref name="request"/> is valid (optional).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected CommandDispatcherCommand(ICommandDispatcher request, IIsValidIndicator isValidIndicator)
            : this(request, isValidIndicator, CommandExecutionOptions.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDispatcherCommand{T}" /> class.
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
        protected CommandDispatcherCommand(ICommandDispatcher request, IIsValidIndicator isValidIndicator, CommandExecutionOptions options)
            : base(request, isValidIndicator, options) { }        
    }
}
