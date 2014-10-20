namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// When implemented by a class, represents a pipeline a message goes through before it is processed or executed.
    /// </summary>
    public interface IMessagePipeline
    {
        /// <summary>
        /// Executes the specified <paramref name="command" /> using the specified <paramref name="processor"/>.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="processor">The processor used to execute the command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> or <paramref name="processor"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="RequestExecutionException">
        /// The specified <paramref name="command"/> could not be executed for functional reasons.
        /// </exception>
        void Execute<TCommand>(TCommand command, IMessageProcessor processor) where TCommand : class, IRequestMessage;

        /// <summary>
        /// Handles the specified <paramref name="event"/> using the specified <paramref name="processor"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to handle.</typeparam>
        /// <param name="event">The event to handle.</param>
        /// <param name="processor">The processor used to handle the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> or <paramref name="processor"/> is <c>null</c>.
        /// </exception>
        void Handle<TEvent>(TEvent @event, IMessageProcessor processor) where TEvent : class;

        /// <summary>
        /// Executes the specified <paramref name="request"/> using the specified <paramref name="query"/> and returns the result.
        /// </summary>
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request to execute.</param>
        /// <param name="query">The function used to execute the request.</param>
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="RequestExecutionException">
        /// The specified <paramref name="request"/> could not be executed for functional reasons.
        /// </exception>
        TResponse Execute<TRequest, TResponse>(TRequest request, Func<TRequest, TResponse> query) where TRequest : class, IRequestMessage;
    }
}
