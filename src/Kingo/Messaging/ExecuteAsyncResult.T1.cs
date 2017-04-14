namespace Kingo.Messaging
{
    /// <summary>
    /// Represents the result of the invocation of a <see cref="IQuery{T}" /> or <see cref="IQuery{T, S}" />.
    /// </summary>
    public abstract class ExecuteAsyncResult<TMessageOut>
    {
        internal ExecuteAsyncResult(TMessageOut message, IMessageStream metadataStream)
        {                                               
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

        /// <inheritdoc />
        public override string ToString() =>
            $"{nameof(Message)} type: {typeof(TMessageOut).FriendlyName()}, {nameof(MetadataStream)}: {MetadataStream.Count} message(s)";

        internal abstract void Commit();
    }
}
