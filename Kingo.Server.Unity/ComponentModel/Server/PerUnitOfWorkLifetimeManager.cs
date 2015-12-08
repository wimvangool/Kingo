using System;
using Kingo.Messaging;
using Microsoft.Practices.Unity;

namespace Kingo.ComponentModel.Server
{
    /// <summary>
    /// A LifetimeManager for Unity that registers it's dependencies at the current <see cref="UnitOfWorkContext" />.
    /// </summary>
    public sealed class PerUnitOfWorkLifetimeManager : LifetimeManager
    {       
        private IDependencyCacheEntry<object> _entry;           

        /// <inheritdoc />
        public override object GetValue()
        {
            object value;

            if (_entry != null && _entry.TryGetValue(out value))
            {
                return value;
            }
            return null;
        }

        /// <inheritdoc />
        public override void RemoveValue()
        {
            if (_entry == null)
            {
                return;
            }
            _entry.Dispose();
            _entry = null;
        }

        /// <inheritdoc />
        public override void SetValue(object newValue)
        {
            IDependencyCache cache;

            if (TryGetDependencyCache(out cache))
            {
                _entry = cache.Add(newValue, HandleValueInvalidated);
            }
            else
            {
                HandleValueInvalidated(newValue);
            }
        }

        private static bool TryGetDependencyCache(out IDependencyCache cache)
        {
            var context = UnitOfWorkContext.Current;
            if (context == null)
            {
                cache = null;
                return false;
            }
            cache = context.Cache;
            return true;
        }

        private static void HandleValueInvalidated(object value)
        {
            var disposable = value as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }        
    }
}
