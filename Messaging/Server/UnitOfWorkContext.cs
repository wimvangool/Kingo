using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Globalization;
using System.Threading;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents the context in which a certain message is being handled.
    /// </summary>
    public sealed class UnitOfWorkContext : IDisposable
    {
        private readonly Stack<BufferedEventBus> _bufferedEventBuses;             
        private readonly UnitOfWorkController _flushController;
        private readonly ScopeSpecificCache _cache;   
        private bool _isDisposed;
        
        internal UnitOfWorkContext()
        {                                    
            _bufferedEventBuses = new Stack<BufferedEventBus>(3);                                
            _flushController = new UnitOfWorkController();
            _cache = new ScopeSpecificCache(); 
        }          
        
        internal IScopeSpecificCache InternalCache
        {
            get { return _cache; }
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

        internal BufferedEventBus PushBus(IDomainEventBus domainEventBus)
        {
            var bufferedEventBus = new BufferedEventBus(domainEventBus);

            _bufferedEventBuses.Push(bufferedEventBus);
            _flushController.Enlist(bufferedEventBus);

            return bufferedEventBus;
        }

        internal IDomainEventBus PeekBus()
        {
            return _bufferedEventBuses.Peek();
        }

        internal BufferedEventBus PopBus()
        {
            return _bufferedEventBuses.Pop();
        }

        internal void EnlistUnitOfWork(IUnitOfWork unitOfWork)
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

        /// <summary>
        /// Represents the cache that is associated to the current <see cref="UnitOfWorkContext" />.
        /// </summary>
        public static readonly IScopeSpecificCache Cache = new UnitOfWorkCache();

        private static readonly ThreadLocal<UnitOfWorkContext> _Current = new ThreadLocal<UnitOfWorkContext>();

        /// <summary>
        /// Returns the context that is being maintained for the message that is currently being handled.
        /// </summary>
        public static UnitOfWorkContext Current
        {
            get { return _Current.Value; }
            internal set { _Current.Value = value; }
        }        

        /// <summary>
        /// Enlist the specified unit of work with the current context, if present.
        /// </summary>
        /// <param name="unitOfWork">Unit of work to enlist.</param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Current"/> has not been set to an instance of an object.
        /// </exception>   
        public static void Enlist(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }
            var context = _Current.Value;
            if (context == null)
            {
                throw NewFailedToEnlistException(unitOfWork.GetType());
            }
            context.EnlistUnitOfWork(unitOfWork);            
        }

        private static Exception NewFailedToEnlistException(Type type)
        {
            var messageFormat = ExceptionMessages.UnitOfWorkContext_FailedToEnlistUnitOfWork;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, type);
            return new InvalidOperationException(message);
        }
    }
}
