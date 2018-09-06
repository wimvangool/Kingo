using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Messaging
{
    internal sealed class UnitOfWorkCache : Disposable, IUnitOfWorkCache
    {
        private readonly Dictionary<string, object> _items;
        private readonly List<object> _removedItems;

        public UnitOfWorkCache()
        {
            _items = new Dictionary<string, object>();
            _removedItems = new List<object>();
        }        

        public object this[string key]
        {
            get
            {
                if (_items.TryGetValue(key, out var item))
                {
                    return item;
                }
                return null;
            }
            set
            {               
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                Remove(key);

                _items.Add(key, value);
            }
        }

        public void Remove(string key)
        {
            if (_items.TryGetValue(key, out var item) && _items.Remove(key))
            {
                _removedItems.Add(item);                
            }            
        }

        protected override void DisposeManagedResources()
        {
            DisposeItems(_items.Values.Concat(_removedItems).WhereNotNull().OfType<IDisposable>());

            base.DisposeManagedResources();
        }

        private static void DisposeItems(IEnumerable<IDisposable> items)
        {
            foreach (var disposable in items)
            {
                disposable.Dispose();
            }
        }
    }
}
