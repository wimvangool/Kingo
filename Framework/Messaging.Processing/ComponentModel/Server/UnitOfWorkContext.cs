using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Syztem.Threading;

namespace Syztem.ComponentModel.Server
{
    /// <summary>
    /// Represents the context in which a certain message is being handled.
    /// </summary>    
    internal sealed class UnitOfWorkContext : IDisposable
    {        
        private readonly Stack<BufferedEventBus> _eventBusStack;             
        private readonly UnitOfWorkController _flushController;
        private readonly DependencyCache _cache;   
        private bool _isDisposed;
        
        internal UnitOfWorkContext()
        {                                    
            _eventBusStack = new Stack<BufferedEventBus>();                                
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
            EventBus.PublishAsync(message).Wait();
        }

        internal void Enlist(IUnitOfWork unitOfWork)
        {            
            _flushController.Enlist(unitOfWork);
        }
        
        internal async Task FlushAsync()
        {
            if (_isDisposed)
            {
                throw NewThreadAlreadyDisposedException();
            }
            await _flushController.FlushAsync();
        }

        private Exception NewThreadAlreadyDisposedException()
        {
            return new ObjectDisposedException(GetType().FullName);
        }                

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly AsyncLocal<UnitOfWorkContext> _Current = new AsyncLocal<UnitOfWorkContext>();

        /// <summary>
        /// Returns the context that is being maintained for the message that is currently being handled.
        /// </summary>
        internal static UnitOfWorkContext Current
        {
            get { return _Current.Value; }
            set { _Current.Value = value; }
        }                
    }
}
