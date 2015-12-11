using System;
using System.Threading.Tasks;
using Kingo.Resources;
using Kingo.Threading;

namespace Kingo.Messaging
{    
    /// <summary>
    /// Represents a scope that controls the lifetime of a <see cref="UnitOfWorkContext" />.
    /// </summary>        
    public sealed class UnitOfWorkScope : IDisposable
    {
        private readonly ContextScope<UnitOfWorkContext> _scope;
        private readonly bool _isContextOwner;
        private bool _hasCompleted;
        private bool _isDisposed;                            
                
        internal UnitOfWorkScope(ContextScope<UnitOfWorkContext> scope, bool isContextOwner)
        {
            _scope = scope;
            _isContextOwner = isContextOwner;
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
        public async Task CompleteAsync()
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
                await _scope.Value.FlushAsync();
            }
            _hasCompleted = true;
        }  

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This scope was incorrectly nested with another scope.
        /// </exception>
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
                _scope.Value.Dispose();
            }
            _scope.Dispose();
            _isDisposed = true;                       
        }                                      

        private static Exception NewScopeAlreadyDisposedException()
        {
            return new ObjectDisposedException(typeof(UnitOfWorkScope).Name);
        }                               

        private static Exception NewScopeAlreadyCompletedException()
        {
            return new InvalidOperationException(ExceptionMessages.TransactionScope_ScopeAlreadyCompleted);
        }        
    }
}

