using System;
using Kingo.Resources;

namespace Kingo.Threading
{
    /// <summary>
    /// Represents a scope that is used to temporarily set the current value of a context.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public abstract class ContextScope<TValue> : IDisposable
    {
        private bool _isDisposed;

        /// <summary>
        /// Returns the value that is managed by this scope.
        /// </summary>
        public abstract TValue Value
        {
            get;
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
            if (RestoreOldValue())
            {
                _isDisposed = true;
                return;
            }
            throw NewIncorrectNestingOfScopesException();
        }

        internal abstract bool RestoreOldValue();

        private static Exception NewIncorrectNestingOfScopesException() => new InvalidOperationException(ExceptionMessages.Scope_IncorrectNesting);
    }
}
