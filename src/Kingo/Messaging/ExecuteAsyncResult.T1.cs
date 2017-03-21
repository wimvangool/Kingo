using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents the result of the invocation of a <see cref="IQuery{T}" /> or <see cref="IQuery{T, S}" />.
    /// </summary>
    public sealed class ExecuteAsyncResult<TMessageOut>
    {
        internal ExecuteAsyncResult(TMessageOut message, IMessageStream metadataStream)
        {                        
            if (metadataStream == null)
            {
                throw new ArgumentNullException(nameof(metadataStream));
            }            
            Message = message;
            MetadataStream = metadataStream;
        }

        /// <summary>
        /// Represents the result of the executed query.
        /// </summary>
        public TMessageOut Message
        {
            get;
        }

        /// <summary>
        /// Represents the stream of events that contain metadata about the application's behavior. All metadata events will be handled
        /// on separate threads by <see cref="IMessageHandler{T}" /> classes that accept message of source <see cref="MessageSources.MetadataStream" />.
        /// </summary>
        public IMessageStream MetadataStream
        {
            get;
        }

        /// <summary>
        /// Replaces the existing <see cref="Message" /> with another message and returns the new result.
        /// </summary>
        /// <param name="message">The new message to return.</param>
        /// <returns>A new result containing the specified <paramref name="message"/>.</returns>                
        public ExecuteAsyncResult<TMessageOut> ReplaceMessage(TMessageOut message) =>
            new ExecuteAsyncResult<TMessageOut>(message, MetadataStream);

        /// <summary>
        /// Replaces the existing <see cref="MetadataStream" /> with another stream and returns the new result.
        /// </summary>
        /// <param name="metadataStream">The new metadata stream to return.</param>
        /// <returns>A new result containing the specified <paramref name="metadataStream"/>.</returns>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="metadataStream"/> is <c>null</c>.
        /// </exception>
        public ExecuteAsyncResult<TMessageOut> ReplaceMetadataStream(IMessageStream metadataStream) =>
            new ExecuteAsyncResult<TMessageOut>(Message, metadataStream);
    }
}
