using System;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// This type is used to support implicit type conversion from a <see cref="Action" /> to a
    /// <see cref="IMessageHandler{TMessage}" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this handler.</typeparam>
    public sealed class MessageHandlerDelegate<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly Func<TMessage, Task> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerDelegate{TMessage}" /> class.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerDelegate(Action<TMessage> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _handler = message => Task.Run(() => handler.Invoke(message));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerDelegate{TMessage}" /> class.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerDelegate(Func<TMessage, Task> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _handler = handler;
        }

        Task IMessageHandler<TMessage>.HandleAsync(TMessage message)
        {
            return _handler.Invoke(message);
        }        
    }
}
