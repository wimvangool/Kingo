using System;
using System.Collections.Generic;
using Kingo.Threading;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a context in which resolved dependencies can be stored,
    /// so that their lifetime is associated to the lifetime of the context.
    /// </summary>
    public class DependencyContext : Disposable, IDependencyContext
    {
        #region [====== Scope ======]

        private sealed class Scope : IDisposable
        {
            private readonly ContextScope<DependencyContext> _dependencyContextScope;
            private readonly IDisposable _otherScope;

            public Scope(ContextScope<DependencyContext> scope, IDisposable otherScope)
            {
                _dependencyContextScope = scope;
                _otherScope = otherScope;
            }

            public void Dispose()
            {
                _otherScope?.Dispose();
                _dependencyContextScope.Value.Dispose();
                _dependencyContextScope.Dispose();
            }
        }

        #endregion                

        internal DependencyContext(IDictionary<Guid, object> dependencies = null)
        {
            Dependencies = dependencies ?? new Dictionary<Guid, object>();
        }
        
        /// <summary>
        /// Returns the values that are stored in this context.
        /// </summary>
        protected IDictionary<Guid, object> Dependencies
        {
            get;
        }

        public override string ToString() =>
            $"{Dependencies.Count} dependency/dependencies stored";

        /// <inheritdoc />
        public void SetValue(Guid key, object value)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (Dependencies.TryGetValue(key, out var storedValue))
            {
                if (ReferenceEquals(value, storedValue))
                {
                    return;
                }
                RemoveValue(key);
            }
            Dependencies.Add(key, value);
        }

        /// <inheritdoc />
        public object GetValue(Guid key)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (Dependencies.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        /// <inheritdoc />
        public bool RemoveValue(Guid key)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (Dependencies.TryGetValue(key, out var storedValue) && storedValue is IDisposable value)
            {
                value.Dispose();
            }
            return Dependencies.Remove(key);
        }

        private DependencyContext CreateChildContext() =>
            new DependencyContext(Dependencies);

        #region [====== Current ======]

        private static readonly Context<DependencyContext> _Context = new Context<DependencyContext>();

        /// <summary>
        /// Returns the currently active context.
        /// </summary>
        public static DependencyContext Current =>
            _Context.Current;

        /// <summary>
        /// Creates and returns a new scope for the <see cref="DependencyContext" /> that will be ended as soon as it's disposed.
        /// </summary>
        /// <param name="nestedScope">Optional other scope that will be nested in inside the newly created scope.</param>
        /// <returns>The scope.</returns>
        public static IDisposable CreateScope(IDisposable nestedScope = null) =>
            new Scope(_Context.OverrideAsyncLocal(CreateContext(Current)), nestedScope);

        private static DependencyContext CreateContext(DependencyContext context) =>
            context?.CreateChildContext() ?? new DependencyContextRoot();

        #endregion
    }
}
