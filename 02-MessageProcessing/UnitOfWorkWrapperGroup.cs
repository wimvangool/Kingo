using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace YellowFlare.MessageProcessing
{    
    internal sealed class UnitOfWorkWrapperGroup : UnitOfWorkWrapper
    {
        private readonly List<UnitOfWorkWrapperItem> _items;                

        public UnitOfWorkWrapperGroup(UnitOfWorkWrapperItem left, UnitOfWorkWrapperItem right)
            : this(new [] { left, right }) {}

        public UnitOfWorkWrapperGroup(IEnumerable<UnitOfWorkWrapperItem> items)
        {
            Debug.Assert(items != null);
            Debug.Assert(items.All(item => item != null));
            Debug.Assert(items.Count() > 1);
            Debug.Assert(items.Select(item => item.Group).Distinct().Count() == 1);

            _items = new List<UnitOfWorkWrapperItem>(items);
            _items.TrimExcess();
        }

        public override string Group
        {
            get { return _items[0].Group; }
        }

        public override bool ForceSynchronousFlush
        {
            get { return _items.Any(item => item.ForceSynchronousFlush); }
        }

        public override bool WrapsSameUnitOfWorkAs(UnitOfWorkWrapperItem item)
        {
            return _items.Any(wrapper => wrapper.WrapsSameUnitOfWorkAs(item));
        }

        protected override UnitOfWorkWrapperGroup MergeWith(UnitOfWorkWrapperItem item)
        {
            _items.Add(item);
            return this;
        }

        public override void CollectWrappersThatRequireFlush(ICollection<UnitOfWorkWrapper> wrappers)
        {
            var itemsThatRequireFlush = _items.Where(item => item.RequiresFlush()).ToArray();
            if (itemsThatRequireFlush.Length == 1)
            {
                wrappers.Add(itemsThatRequireFlush[0]);                
            }
            else if (itemsThatRequireFlush.Length == _items.Count)
            {
                wrappers.Add(this);
            }
            else if (itemsThatRequireFlush.Length > 1)
            {
                wrappers.Add(new UnitOfWorkWrapperGroup(itemsThatRequireFlush));
            }
        }

        public override void Flush()
        {
            foreach (var item in _items)
            {
                item.Flush();
            }
        }                    
    }
}
