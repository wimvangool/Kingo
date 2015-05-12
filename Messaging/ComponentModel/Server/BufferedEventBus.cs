using System.Collections.Generic;
using System.Diagnostics;
using System.Resources;

namespace System.ComponentModel.Server
{    
    internal sealed class BufferedEventBus : IDomainEventBus, IUnitOfWork
    {
        private readonly MessageProcessor _processor;        
        private readonly UnitOfWorkContext _context;
        private readonly List<IEventBuffer> _buffer;
        
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int IUnitOfWork.FlushGroupId
        {
            get { return 0; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IUnitOfWork.CanBeFlushedAsynchronously
        {
            get { return false; }
        }

        void IDomainEventBus.Publish<TMessage>(TMessage message)
        {
            var messageCopy = message.Copy();
            var errorTree = messageCopy.Validate();
            if (errorTree.TotalErrorCount > 0)
            {
                throw NewInvalidEventException("message", messageCopy, errorTree);
            }
            _buffer.Add(new EventBuffer<TMessage>(_processor.EventBus, message));
            _context.Enlist(this);
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
