using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging
{    
    internal sealed class UnitOfWorkBus : IUnitOfWork
    {
        private readonly UnitOfWorkContext _context;
        private List<IEventToPublish> _buffer;
        
        internal UnitOfWorkBus(UnitOfWorkContext context)
        {
            _context = context;
            _buffer = new List<IEventToPublish>();                       
        }       

        public void Publish<TEvent>(TEvent @event) where TEvent : class, IMessage
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            var eventCopy = (TEvent) @event.Copy();

            var errorInfo = eventCopy.Validate();
            if (errorInfo.HasErrors)
            {
                throw NewInvalidEventException(nameof(@event), eventCopy, errorInfo);
            }
            lock (_buffer)
            {
                _buffer.Add(new EventToPublish<TEvent>(eventCopy));
            }
            _context.Enlist(this);
        }

        bool IUnitOfWork.RequiresFlush()
        {
            return _buffer.Count > 0;
        }

        Task IUnitOfWork.FlushAsync()
        {
            return FlushAsync(Interlocked.Exchange(ref _buffer, new List<IEventToPublish>()));
        }

        private async Task FlushAsync(IEnumerable<IEventToPublish> events)
        {
            foreach (var bufferedEvent in events)
            {
                await bufferedEvent.PublishAsync(_context.Processor.EventBus);
            }  
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{_buffer.Count} Event(s) Published";
        }

        private static Exception NewInvalidEventException(string paramName, IMessage invalidEvent, ErrorInfo errorInfo)
        {
            var messageFormat = ExceptionMessages.DomainEventBus_InvalidMessage;
            var message = string.Format(messageFormat, invalidEvent.GetType().Name);
            return new InvalidEventException(paramName, invalidEvent, message, errorInfo);
        }
    }
}
