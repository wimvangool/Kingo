using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents the context in which a certain command is being executed.
    /// </summary>
    public sealed class UnitOfWorkContext : IDisposable
    {
        private readonly Stack<BufferedEventBus> _bufferedEventBuses;
        private readonly Lazy<UnitOfWorkContextCache> _cache;        
        private readonly UnitOfWorkController _flushController;        
        private bool _isDisposed;
        
        internal UnitOfWorkContext()
        {                                    
            _bufferedEventBuses = new Stack<BufferedEventBus>(2);            
            _cache = new Lazy<UnitOfWorkContextCache>(() => new UnitOfWorkContextCache());          
            _flushController = new UnitOfWorkController();            
        }        

        /// <summary>
        /// Returns a cache that can be used to store items that have a lifecycle of single command execution.
        /// </summary>
        public IDictionary<Guid, object> Cache
        {
            get { return _cache.Value; }
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
        /// <returns><c>true</c> if the unit of work was enlisted; otherwise <c>false</c>.</returns>        
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
