using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a message handler that can handle a specific set of messages.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// The message types that are supported by this message handler.
        /// </summary>
        IReadOnlyCollection<Type> MessageTypes
        {
            get;
        }


        Task HandleAsync(object message, CancellationToken token);
    }
}
