using System.Security.Principal;
using System.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents the context in which a message or <see cref="IMessageStream" /> is being processed by a <see cref="IMicroProcessor" />.
    /// </summary>
    public interface IMicroProcessorContext
    {        
        /// <summary>
        /// Returns the <see cref="IPrincipal" /> that was retrieved from the <see cref="MicroProcessor" /> that is processing this message.
        /// </summary>
        IPrincipal Principal
        {
            get;
        }

        /// <summary>
        /// Returns message-info about the message that is currently being handled or executed by the processor.
        /// </summary>
        IMessageStackTrace Messages
        {
            get;
        }

        /// <summary>
        /// Returns the 
        /// </summary>
        IUnitOfWorkController UnitOfWork
        {
            get;
        }

        /// <summary>
        /// Represents the stream of events that is published during a unit of work. All output event will be handled by
        /// <see cref="IMessageHandler{T}" /> classes that accept message of source <see cref="MessageSources.OutputStream" />.
        /// </summary>
        IEventStream OutputStream
        {
            get;
        }

        /// <summary>
        /// Represents the stream of events that contain metadata about the application's behavior. All metadata events will be handled
        /// on separate threads by <see cref="IMessageHandler{T}" /> classes that accept message of source <see cref="MessageSources.MetadataStream" />.
        /// </summary>
        IEventStream MetadataStream
        {
            get;
        }

        /// <summary>
        /// Returns the <see cref="CancellationToken"/> that was passed to the <see cref="IMicroProcessor" />.
        /// </summary>
        CancellationToken Token
        {
            get;
        }               
    }
}
