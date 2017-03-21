using System;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents the result of the invocation of a <see cref="IMessageHandler{T}" />.
    /// </summary>
    public sealed class HandleAsyncResult
    {        
        internal HandleAsyncResult(bool isMetadataResult, IMessageStream outputStream, IMessageStream metadataStream)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }
            if (metadataStream == null)
            {
                throw new ArgumentNullException(nameof(metadataStream));
            }
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

        /// <summary>
        /// Replaces the existing <see cref="OutputStream" /> with another stream and returns the new result.
        /// </summary>
        /// <param name="outputStream">The new output stream to return.</param>
        /// <returns>A new result containing the specified <paramref name="outputStream"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsMetadataResult" /> is <c>true</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputStream"/> is <c>null</c>.
        /// </exception>
        public HandleAsyncResult ReplaceOutputStream(IMessageStream outputStream)
        {
            if (IsMetadataResult)
            {
                throw NewCannotReplaceOutputStreamException();
            }
            return new HandleAsyncResult(IsMetadataResult, outputStream, MetadataStream);
        }

        /// <summary>
        /// Replaces the existing <see cref="MetadataStream" /> with another stream and returns the new result.
        /// </summary>
        /// <param name="metadataStream">The new output stream to return.</param>
        /// <returns>A new result containing the specified <paramref name="metadataStream"/>.</returns>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="metadataStream"/> is <c>null</c>.
        /// </exception>
        public HandleAsyncResult ReplaceMetadataStream(IMessageStream metadataStream) =>
            new HandleAsyncResult(IsMetadataResult, OutputStream, metadataStream);

        private static Exception NewCannotReplaceOutputStreamException() =>
            new InvalidOperationException(ExceptionMessages.HandleAsyncResult_CannotReplaceOutputStream);
    }
}
