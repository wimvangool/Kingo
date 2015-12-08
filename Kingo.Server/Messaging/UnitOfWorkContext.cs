using System;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents the context in which a certain message is being handled.
    /// </summary>    
    public sealed class UnitOfWorkContext : IDisposable
    {
        private readonly MessageProcessor _processor;
        private readonly DomainEventBus _eventBus;
        private readonly UnitOfWorkController _flushController;
        private readonly DependencyCache _cache;   
        private bool _isDisposed;
        
        internal UnitOfWorkContext(MessageProcessor processor)
        {
            _processor = processor;
            _eventBus = new DomainEventBus(this);
            _flushController = new UnitOfWorkController();
            _cache = new DependencyCache(); 
        }          
        
        internal MessageProcessor Processor
        {
            get { return _processor; }
        }
        
        /// <summary>
        /// Returns the cache associated with this unit of work.
        /// </summary>
        public IDependencyCache Cache
        {
            get { return _cache; }
        }  
        
        /// <summary>
        /// Publishes the specified <paramref name="message"/> as soon as this unit of work is flushed.
        /// </summary>
        /// <typeparam name="TMessage">Type of the event to publish.</typeparam>
        /// <param name="message">The event to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="message"/> is not valid.
        /// </exception>
        public void Publish<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            _eventBus.Publish(message);
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

        /// <summary>
        /// Schedules the specified <paramref name="unitOfWork"/> for a flush.
        /// </summary>
        /// <param name="unitOfWork">The unit of work to enlist.</param>
        /// <exception cref="ObjectDisposedException">
        /// The context has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="unitOfWork"/> is <c>null</c>.
        /// </exception>
        public void Enlist(IUnitOfWork unitOfWork)
        {
            if (_isDisposed)
            {
                throw NewThreadAlreadyDisposedException();
            }
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

        #region [====== Current ======]

        private static readonly Context<UnitOfWorkContext> _Context = new Context<UnitOfWorkContext>();

        /// <summary>
        /// Returns the current context.
        /// </summary>
        public static UnitOfWorkContext Current
        {
            get { return _Context.Current; }            
        }      
        
        internal static UnitOfWorkScope StartUnitOfWorkScope(MessageProcessor processor)
        {
            // A new UnitOfWorkContext is only created if it doesn't already exist. Note that
            // the new scope is always set on ThreadLocal storage, even though we are supporting
            // asynchronous operations, because the MessageProcessor ensures all continuations
            // are handled on the same thread.
            var isContextOwner = Current == null;
            var context = isContextOwner ? new UnitOfWorkContext(processor) : Current;
            var scope = _Context.OverrideThreadLocal(context);

            return new UnitOfWorkScope(scope, isContextOwner);
        }

        #endregion
    }
}
