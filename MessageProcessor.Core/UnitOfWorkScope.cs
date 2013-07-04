using System;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{    
    /// <summary>
    /// Represents a scope that controls the lifetime of a <see cref="UnitOfWorkContext" />.
    /// </summary>        
    public sealed class UnitOfWorkScope : IDisposable
    {        
        private readonly bool _isContextOwner;
        private bool _hasCompleted;
        private bool _isDisposed;
                    
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkScope" /> class.
        /// </summary>
        public UnitOfWorkScope()
        {
            var currentUnitOfWork = UnitOfWorkContext.Current;            
            if (currentUnitOfWork == null)
            {
                UnitOfWorkContext.Current = new UnitOfWorkContext();

                _isContextOwner = true;
            }                
        }              

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>        
        /// <remarks>
        /// The Dispose()-method marks the end of the current scope's lifetime and will set
        /// <see cref="UnitOfWorkContext.Current" /> back to <c>null</c> after disposing it.
        /// </remarks>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }            
            if (_isContextOwner)
            {                
                UnitOfWorkContext.Current.Dispose();
                UnitOfWorkContext.Current = null;
            }
            _isDisposed = true;                       
        }

        /// <summary>
        /// Completes the scope by flushing all registered wrappers.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The scope has already been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The scope has already been completed.
        /// </exception>
        public void Complete()
        {
            if (_isDisposed)
            {
                throw NewScopeAlreadyDisposedException();
            }
            if (_hasCompleted)
            {
                throw NewScopeAlreadyCompletedException();
            }
            if (_isContextOwner)
            {
                UnitOfWorkContext.Current.Flush();
            }
            _hasCompleted = true;                                       
        }

        private Exception NewScopeAlreadyDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }                               

        private static Exception NewScopeAlreadyCompletedException()
        {
            return new InvalidOperationException(ExceptionMessages.UnitOfWorkContext_ScopeAlreadyCompleted);
        }
    }
}

