using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Globalization;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents an in-memory event-bus that is flushed when the <see cref="UnitOfWorkScope" /> completes.
    /// </summary>
    public sealed class BufferedEventBus : IDomainEventBus, IUnitOfWork
    {        
        private readonly List<IEventBuffer> _buffer;
        private readonly IDomainEventBus _domainEventBus;
        private readonly UnitOfWorkContext _context;

        internal BufferedEventBus(IDomainEventBus domainEventBus, UnitOfWorkContext context)
        {                        
            _buffer = new List<IEventBuffer>();
            _domainEventBus = domainEventBus;
            _context = context;
        }

        Guid IUnitOfWork.FlushGroupId
        {
            get { return Guid.Empty; }
        }

        bool IUnitOfWork.CanBeFlushedAsynchronously
        {
            get { return false; }
        }

        void IDomainEventBus.Publish<TMessage>(TMessage message)
        {            
            _buffer.Add(new EventBuffer<TMessage>(_domainEventBus, message));
            _context.EnlistUnitOfWork(this);
        }

        bool IUnitOfWork.RequiresFlush()
        {
            return _buffer.Count > 0;
        }

        void IUnitOfWork.Flush()
        {
            foreach (var bufferedEvent in _buffer)
            {
                bufferedEvent.Flush();
            }            
            _buffer.Clear();            
        }

        /// <summary>
        /// Publishes the specified event on the bus that is currently in scope.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to publish.</typeparam>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="InvalidOperationException">
        /// No instance is currrently in scope on which the message can be published.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message" /> is <c>null</c>.
        /// </exception>
        public static void Publish<TMessage>(TMessage message) where TMessage : class
        {
            var context = UnitOfWorkContext.Current;
            if (context != null)
            {
                context.PeekBus().Publish(message);
                return;
            }
            throw NewNoBusAvailableException(typeof(TMessage));            
        }

        private static Exception NewNoBusAvailableException(Type messageType)
        {
            var messageFormat = ExceptionMessages.BufferedEventBus_NoBusAvailable;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, messageType.Name);
            return new InvalidOperationException(message);
        }        
    }
}
