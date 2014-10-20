namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IMessagePipeline" /> interface.
    /// </summary>
    public class MessagePipeline : IMessagePipeline
    {
        /// <inheritdoc />
        /// <remarks>
        /// This method will validate the <paramref name="command"/> before it is executed.
        /// </remarks>
        public virtual void Execute<TCommand>(TCommand command, IMessageProcessor processor) where TCommand : class, IRequestMessage           
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            if (command != null)
            {
                command.Validate();

                if (command.IsValid)
                {                    
                    processor.Process(command);
                    return;
                }
                throw new InvalidMessageException(command);
            }
            throw new ArgumentNullException("command");
        }

        /// <inheritdoc />
        public virtual void Handle<TEvent>(TEvent @event, IMessageProcessor processor) where TEvent : class
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            processor.Process(@event);
        }

        /// <inheritdoc />
        /// <remarks>
        /// This method will validate the <paramref name="request"/> before it is executed.
        /// </remarks>
        public virtual TResponse Execute<TRequest, TResponse>(TRequest request, Func<TRequest, TResponse> query) where TRequest : class, IRequestMessage
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (request != null)
            {
                request.Validate();

                if (request.IsValid)
                {
                    return query.Invoke(request);
                }
                throw new InvalidMessageException(request);
            }
            throw new ArgumentNullException("request");
        }
    }
}
