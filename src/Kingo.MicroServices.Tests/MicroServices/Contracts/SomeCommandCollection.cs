using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kingo.MicroServices.Contracts
{
    [Serializable]
    public sealed class SomeCommandCollection : IReadOnlyCollection<SomeCommandCollection>
    {
        private readonly List<SomeCommandCollection> _items;
        private object _propertyA;

        public SomeCommandCollection(object propertyA = null)
        {
            _items = new List<SomeCommandCollection>();
            _propertyA = propertyA;
        }

        [Required]
        public object PropertyA =>
            _propertyA;

        public int Count =>
            _items.Count;

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<SomeCommandCollection> GetEnumerator() =>
            _items.GetEnumerator();

        public void Add(SomeCommandCollection collection) =>
            _items.Add(collection);
    }
}
