using System;

namespace YellowFlare.MessageProcessing
{    
    /// <summary>
    /// Represents a scope that controls the lifetime of a <see cref="MessageProcessorContext" />.
    /// </summary>        
    internal sealed class MessageProcessorContextScope : IDisposable
    {        
        private readonly bool _isContextOwner;
        private bool _hasCompleted;
        private bool _isDisposed;
                    
        internal MessageProcessorContextScope(MessageProcessor processor)
        {
            var currentUnitOfWork = MessageProcessorContext.Current;            
            if (currentUnitOfWork == null)
            {
                MessageProcessorContext.Current = new MessageProcessorContext(processor);

                _isContextOwner = true;
            }                
        }              

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>        
        /// <remarks>
        /// The Dispose()-method marks the end of the current scope's lifetime and will set
        /// <see cref="MessageProcessorContext.Current" /> back to <c>null</c> after disposing it.
        /// </remarks>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }            
            if (_isContextOwner)
            {                
                MessageProcessorContext.Current.Dispose();
                MessageProcessorContext.Current = null;
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
                MessageProcessorContext.Current.Flush();
            }
            _hasCompleted = true;                                       
        }

        private Exception NewScopeAlreadyDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }                               

        private static Exception NewScopeAlreadyCompletedException()
        {
            return new InvalidOperationException(ExceptionMessages.MessageProcessorContext_ScopeAlreadyCompleted);
        }
    }
}

