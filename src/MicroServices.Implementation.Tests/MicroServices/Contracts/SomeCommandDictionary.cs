using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices.Contracts
{
    public sealed class SomeCommandDictionary : IReadOnlyCollection<KeyValuePair<string, SomeCommandDictionary>>
    {
        private readonly Dictionary<string, SomeCommandDictionary> _items;
        private readonly object _propertyA;

        public SomeCommandDictionary(object propertyA = null)
        {
            _items = new Dictionary<string, SomeCommandDictionary>();
            _propertyA = propertyA;
        }

        [Required]
        public object PropertyA =>
            _propertyA;

        public int Count =>
            _items.Count;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<KeyValuePair<string, SomeCommandDictionary>> GetEnumerator() =>
            _items.GetEnumerator();

        public void Add(string key, SomeCommandDictionary value) =>
            _items.Add(key, value);        
    }
}
