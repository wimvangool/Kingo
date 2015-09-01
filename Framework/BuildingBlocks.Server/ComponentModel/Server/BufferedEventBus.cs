using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Resources;
using Kingo.BuildingBlocks.Threading;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{    
    internal sealed class BufferedEventBus : IDomainEventBus, IUnitOfWork
    {        
        private readonly MessageProcessor _processor;        
        private readonly UnitOfWorkContext _context;
        private List<IEventBuffer> _buffer;
        
        internal BufferedEventBus(MessageProcessor processor, UnitOfWorkContext context)
        {
            _processor = processor;
            _context = context;
            _buffer = new List<IEventBuffer>();                       
        }

        internal MessageProcessor Processor
        {
            get { return _processor; }
        }        

        Task IDomainEventBus.PublishAsync<TMessage>(TMessage message)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                var messageCopy = message.Copy();
                var errorTree = messageCopy.Validate();
                if (errorTree.TotalErrorCount > 0)
                {
                    throw NewInvalidEventException("message", messageCopy, errorTree);
                }
                lock (_buffer)
                {
                    _buffer.Add(new EventBuffer<TMessage>(_processor.EventBus, message));
                }                
                _context.Enlist(this);
            });  
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

        private static Exception NewInvalidEventException(string paramName, IMessage invalidEvent, ValidationErrorTree errorTree)
        {
            var messageFormat = ExceptionMessages.BufferedEventBus_InvalidMessage;
            var message = string.Format(messageFormat, invalidEvent.GetType().Name);
            return new InvalidEventException(paramName, invalidEvent, message, errorTree);
        }
    }
}
