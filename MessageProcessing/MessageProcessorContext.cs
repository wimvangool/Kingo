using System;
using System.Collections.Generic;
using System.Threading;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents the context in which a certain command is being executed.
    /// </summary>
    public sealed class MessageProcessorContext : IUnitOfWorkManager, IDisposable
    {        
        private readonly MessageProcessor _processor;        
        private readonly Lazy<MessageProcessorContextCache> _cache;        
        private readonly FlushController _flushController;
        private readonly Stack<Message> _messageStack;
        private bool _isDisposed;
        
        internal MessageProcessorContext(MessageProcessor processor)
        {                        
            _processor = processor;
            _cache = new Lazy<MessageProcessorContextCache>(() => new MessageProcessorContextCache());          
            _flushController = new FlushController();
            _messageStack = new Stack<Message>();
        }

        /// <summary>
        /// Returns a cache that can be used to store items that have a lifecycle of single command execution.
        /// </summary>
        public IDictionary<Guid, object> Cache
        {
            get { return _cache.Value; }
        }

        internal MessageProcessor MessageProcessor
        {
            get { return _processor; }
        }        

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            if (_cache.IsValueCreated)
            {
                _cache.Value.Dispose();
            }            
            _isDisposed = true;
        } 

        internal void PushMessage(Message message)
        {
            _messageStack.Push(message);
        }

        internal Message PeekMessage()
        {
            return _messageStack.Peek();
        }

        internal void PopMessage()
        {
            _messageStack.Pop();
        }

        void IUnitOfWorkManager.Enlist(IUnitOfWork unitOfWork)
        {
            if (_isDisposed)
            {
                throw NewThreadAlreadyDisposedException();
            }
            _flushController.Register(unitOfWork);
        }
        
        internal void Flush()
        {
            if (_isDisposed)
            {
                throw NewThreadAlreadyDisposedException();
            }
            _flushController.Flush();
        }

        private Exception NewThreadAlreadyDisposedException()
        {
            return new ObjectDisposedException(GetType().FullName);
        }

        private static readonly ThreadLocal<MessageProcessorContext> _Current = new ThreadLocal<MessageProcessorContext>();

        /// <summary>
        /// Returns the context that is being maintained for the message that is currently being handled.
        /// </summary>
        public static MessageProcessorContext Current
        {
            get { return _Current.Value; }
            internal set { _Current.Value = value; }
        }        
    }
}
