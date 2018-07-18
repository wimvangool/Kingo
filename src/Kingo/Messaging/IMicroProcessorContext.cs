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
        /// Returns a <see cref="IClaimsProvider" /> that is based on the <see cref="Principal" /> of this context.
        /// </summary>
        IClaimsProvider ClaimsProvider
        {
            get;
        }

        /// <summary>
        /// Returns the logical call-stack, containing information about the messages that are currently being handled or executed by the processor.
        /// </summary>
        IMessageStackTrace StackTrace
        {
            get;
        }

        /// <summary>
        /// Returns the associated unit of work controller.
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
        /// Returns the <see cref="CancellationToken"/> that was passed to the <see cref="IMicroProcessor" />.
        /// </summary>
        CancellationToken Token
        {
            get;
        }               
    }
}
