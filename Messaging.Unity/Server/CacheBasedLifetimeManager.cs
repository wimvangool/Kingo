using Microsoft.Practices.Unity;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// A LifetimeManager for Unity that registers it's dependencies at the current <see cref="UnitOfWorkContext" />.
    /// </summary>
    public sealed class CacheBasedLifetimeManager : LifetimeManager
    {
        private readonly IDependencyCache _cache;
        private IDependencCacheEntry<object> _entry;    
    
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheBasedLifetimeManager" /> class.
        /// </summary>
        /// <param name="cache">The cache that is used to store all values.</param>
        public CacheBasedLifetimeManager(IDependencyCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
        }

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
            _entry = _cache.Add(newValue, HandleValueInvalidated);
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
