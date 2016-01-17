using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Resources;
using Kingo.Threading;

namespace Kingo.Messaging
{    
    internal sealed class DomainEventBus : IDomainEventBus, IUnitOfWork
    {
        private readonly UnitOfWorkContext _context;
        private List<IEventBuffer> _buffer;
        
        internal DomainEventBus(UnitOfWorkContext context)
        {
            _context = context;
            _buffer = new List<IEventBuffer>();                       
        }               

        Task IDomainEventBus.PublishAsync<TMessage>(TMessage message)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                Publish(message);
            });  
        } 
       
        internal void Publish<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var messageCopy = (TMessage) message.Copy();

            var errorInfo = messageCopy.Validate();
            if (errorInfo.HasErrors)
            {
                throw NewInvalidEventException("message", messageCopy, errorInfo);
            }
            lock (_buffer)
            {
                _buffer.Add(new EventBuffer<TMessage>(_context.Processor.EventBus, messageCopy));
            }
            _context.Enlist(this);
        }

        bool IUnitOfWork.RequiresFlush()
        {
            return _buffer.Count > 0;
        }

        Task IUnitOfWork.FlushAsync()
        {
            return FlushAsync(Interlocked.Exchange(ref _buffer, new List<IEventBuffer>()));
        }

        private static async Task FlushAsync(IEnumerable<IEventBuffer> events)
        {
            foreach (var bufferedEvent in events)
            {
                await bufferedEvent.FlushAsync();
            }  
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} Event(s) Published", _buffer.Count);
        }

        private static Exception NewInvalidEventException(string paramName, IMessage invalidEvent, ErrorInfo errorInfo)
        {
            var messageFormat = ExceptionMessages.BufferedEventBus_InvalidMessage;
            var message = string.Format(messageFormat, invalidEvent.GetType().Name);
            return new InvalidEventException(paramName, invalidEvent, message, errorInfo);
        }
    }
}
