using System;
using System.Collections.Generic;
using System.Threading;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents the context in which a certain command is being executed.
    /// </summary>
    public sealed class UnitOfWorkContext : IUnitOfWorkController, IDisposable
    {
        #region [====== Controller ======]

        private sealed class ControllerImplementation : IUnitOfWorkController
        {            
            public void Enlist(IUnitOfWork unitOfWork)
            {
                var controller = Current as IUnitOfWorkController;
                if (controller == null)
                {
                    throw NewFailedToEnlistUnitOfWorkException();
                }
                controller.Enlist(unitOfWork);
            }

            private static Exception NewFailedToEnlistUnitOfWorkException()
            {
                return new InvalidOperationException(ExceptionMessages.MessageProcessor_FailedToEnlistUnitOfWork);
            }
        }

        #endregion

        private readonly Lazy<UnitOfWorkContextCache> _cache;        
        private readonly UnitOfWorkController _flushController;        
        private bool _isDisposed;
        
        internal UnitOfWorkContext()
        {                                    
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

        void IUnitOfWorkController.Enlist(IUnitOfWork unitOfWork)
        {
            if (_isDisposed)
            {
                throw NewThreadAlreadyDisposedException();
            }
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
        /// Returns the controller that can be used to enlist unit's of work to the ambient context.
        /// </summary>
        public static readonly IUnitOfWorkController Controller = new ControllerImplementation();

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
