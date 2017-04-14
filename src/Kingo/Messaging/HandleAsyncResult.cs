namespace Kingo.Messaging
{
    /// <summary>
    /// Represents the result of the invocation of a <see cref="IMessageHandler{T}" />.
    /// </summary>
    public abstract class HandleAsyncResult
    {        
        internal HandleAsyncResult(bool isMetadataResult, IMessageStream outputStream, IMessageStream metadataStream)
        {            
            IsMetadataResult = isMetadataResult;
            OutputStream = outputStream;
            MetadataStream = metadataStream;
        }

        /// <summary>
        /// Indicates whether or not this result was obtained by invoking a metadata event handler.
        /// </summary>
        public bool IsMetadataResult
        {
            get;
        }

        /// <summary>
        /// Represents the stream of events that is published during a unit of work. All output event will be handled by
        /// <see cref="IMessageHandler{T}" /> classes that accept message of source <see cref="MessageSources.OutputStream" />.
        /// </summary>
        public IMessageStream OutputStream
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
            $"{nameof(OutputStream)}: {OutputStream.Count} message(s), {nameof(MetadataStream)}: {MetadataStream.Count} message(s)";

        internal abstract HandleAsyncResult RemoveOutputStream();

        internal abstract void Commit();
    }
}
