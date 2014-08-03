namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a query executed from a client that lies on top of a regular <see cref="IQueryDispatcher{T}">Query</see>.
    /// </summary>
    /// <typeparam name="TParameter">Type of the parameter that can be specified for executing this request.</typeparam>
    /// <typeparam name="TResult">Type of the result of the associated query.</typeparam>
    public abstract class QueryDispatcherCommand<TParameter, TResult> : RequestDispatcherCommand<IQueryDispatcher<TResult>, TParameter>       
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDispatcherCommand{T, S}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected QueryDispatcherCommand(IQueryDispatcher<TResult> request)
            : this(request, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDispatcherCommand{T, S}" /> class.
        /// </summary>
        /// <param name="request">The encapsulated query.</param>
        /// <param name="isValidIndicator">
        /// The indicator used to indicate whether or not the associated <paramref name="request"/> is valid (optional).
        /// </param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected QueryDispatcherCommand(IQueryDispatcher<TResult> request, IIsValidIndicator isValidIndicator)
            : this(request, isValidIndicator, CommandExecutionOptions.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDispatcherCommand{T, S}" /> class.
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
        protected QueryDispatcherCommand(IQueryDispatcher<TResult> request, IIsValidIndicator isValidIndicator, CommandExecutionOptions options)
            : base(request, isValidIndicator, options) { }
    }
}
