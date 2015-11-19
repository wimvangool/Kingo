using System;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging
{    
    /// <summary>
    /// Represents a scope that controls the lifetime of a <see cref="UnitOfWorkContext" />.
    /// </summary>        
    internal sealed class UnitOfWorkScope : IDisposable
    {
        private readonly UnitOfWorkContext _context;
        private readonly BufferedEventBus _eventBus;
        private readonly bool _isContextOwner;
        private bool _hasCompleted;
        private bool _isDisposed;                            
                
        internal UnitOfWorkScope(MessageProcessor processor)
        {                      
            _isContextOwner = StartScope(out _context);
            _eventBus = _context.StartScope(processor);
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
            EndScope(this);

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
        internal async Task CompleteAsync()
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
                await _context.FlushAsync();                
            }
            _hasCompleted = true;                                       
        }        

        private static bool StartScope(out UnitOfWorkContext context)
        {
            context = UnitOfWorkContext.Current;

            if (context == null)
            {
                context = UnitOfWorkContext.Current = new UnitOfWorkContext();
                return true;
            }
            return false;
        }

        private static void EndScope(UnitOfWorkScope scope)
        {            
            var eventBus = scope._context.EndScope();
            if (eventBus != scope._eventBus)
            {
                throw NewIncorrectNestingOrWrongThreadException();
            }
            if (scope._isContextOwner)
            {
                scope._context.Dispose();

                UnitOfWorkContext.Current = null;
            }
        }

        private static Exception NewScopeAlreadyDisposedException()
        {
            return new ObjectDisposedException(typeof(UnitOfWorkScope).Name);
        }                               

        private static Exception NewScopeAlreadyCompletedException()
        {
            return new InvalidOperationException(ExceptionMessages.TransactionScope_ScopeAlreadyCompleted);
        }

        private static Exception NewIncorrectNestingOrWrongThreadException()
        {
            return new InvalidOperationException(ExceptionMessages.UnitOfWorkScope_IncorrectNestingOrWrongThread);
        }
    }
}

