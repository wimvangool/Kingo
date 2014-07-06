using System;
using System.Collections.Generic;
using System.Globalization;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents an in-memory event-bus that is flushed when the <see cref="UnitOfWorkScope" /> completes.
    /// </summary>
    public sealed class BufferedEventBus : IDomainEventBus, IUnitOfWork
    {        
        private readonly List<IBufferedEvent> _buffer;
        private readonly IDomainEventBus _domainEventBus;
        private bool _hasBeenFlushed;

        internal BufferedEventBus(IDomainEventBus domainEventBus)
        {                        
            _buffer = new List<IBufferedEvent>();
            _domainEventBus = domainEventBus;
        }

        string IUnitOfWork.FlushGroup
        {
            get { return typeof(BufferedEventBus).FullName; }
        }

        bool IUnitOfWork.CanBeFlushedAsynchronously
        {
            get { return false; }
        }

        void IDomainEventBus.Publish<TMessage>(TMessage message)
        {
            if (_hasBeenFlushed)
            {
                throw NewBufferHasAlreadyBeenFlushedException(typeof(TMessage));
            }
            _buffer.Add(new BufferedEvent<TMessage>(message));
        }

        bool IUnitOfWork.RequiresFlush()
        {
            return !_hasBeenFlushed && _buffer.Count > 0;
        }

        void IUnitOfWork.Flush()
        {
            foreach (var bufferedEvent in _buffer)
            {
                bufferedEvent.Publish(_domainEventBus);
            }
            _hasBeenFlushed = true;
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

        private static Exception NewBufferHasAlreadyBeenFlushedException(Type messageType)
        {
            var messageFormat = ExceptionMessages.BufferedEventBus_BusAlreadyFlushed;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, messageType.Name);
            return new InvalidOperationException(message);
        }
    }
}
