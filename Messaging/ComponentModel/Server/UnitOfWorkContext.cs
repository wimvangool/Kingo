using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents the context in which a certain message is being handled.
    /// </summary>
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    public sealed class UnitOfWorkContext : IDisposable
    {
        #region [====== DebuggerProxy ======]

        private sealed class DebuggerProxy
        {
            private readonly UnitOfWorkContext _context;

            internal DebuggerProxy(UnitOfWorkContext context)
            {
                _context = context;
            }

            public BufferedEventBus DomainEventBus
            {
                get { return _context._eventBusStack.Peek(); }
            }

            public UnitOfWorkController FlushController
            {
                get { return _context._flushController; }
            }

            public DependencyCache Cache
            {
                get { return _context._cache; }
            }
        }

        #endregion

        private readonly Stack<BufferedEventBus> _eventBusStack;             
        private readonly UnitOfWorkController _flushController;
        private readonly DependencyCache _cache;   
        private bool _isDisposed;
        
        internal UnitOfWorkContext()
        {                                    
            _eventBusStack = new Stack<BufferedEventBus>(3);                                
            _flushController = new UnitOfWorkController();
            _cache = new DependencyCache(); 
        }          
        
        internal IDependencyCache Cache
        {
            get { return _cache; }
        }
 
        internal MessageProcessor Processor
        {
            get { return _eventBusStack.Peek().Processor; }
        }

        internal IDomainEventBus EventBus
        {
            get { return _eventBusStack.Peek(); }
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
            _cache.Dispose();           
            _isDisposed = true;
        }

        internal BufferedEventBus StartScope(MessageProcessor processor)
        {            
            var eventBus = new BufferedEventBus(processor, this);

            _eventBusStack.Push(eventBus);

            return eventBus;
        }        

        internal BufferedEventBus EndScope()
        {
            return _eventBusStack.Pop();
        }

        internal void Publish<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {            
            EventBus.Publish(message);
        }

        internal void Enlist(IUnitOfWork unitOfWork)
        {            
            _flushController.Enlist(unitOfWork);
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly ThreadLocal<UnitOfWorkContext> _Current = new ThreadLocal<UnitOfWorkContext>();

        /// <summary>
        /// Returns the context that is being maintained for the message that is currently being handled.
        /// </summary>
        public static UnitOfWorkContext Current
        {
            get { return _Current.Value; }
            internal set { _Current.Value = value; }
        }                
    }
}
