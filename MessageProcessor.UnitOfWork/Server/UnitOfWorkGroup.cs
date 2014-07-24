using System.Collections.Generic;
using System.Linq;

namespace YellowFlare.MessageProcessing.Server
{    
    internal sealed class UnitOfWorkGroup : UnitOfWorkWrapper
    {
        private readonly List<UnitOfWorkItem> _items;                

        public UnitOfWorkGroup(UnitOfWorkItem left, UnitOfWorkItem right)
            : this(new [] { left, right }) {}

        public UnitOfWorkGroup(IEnumerable<UnitOfWorkItem> items)
        {            
            _items = new List<UnitOfWorkItem>(items);
            _items.TrimExcess();
        }

        public override string FlushGroup
        {
            get { return _items[0].FlushGroup; }
        }

        public override bool CanBeFlushedAsynchronously
        {
            get { return _items.All(item => item.CanBeFlushedAsynchronously); }
        }

        public override bool WrapsSameUnitOfWorkAs(UnitOfWorkItem item)
        {
            return _items.Any(wrapper => wrapper.WrapsSameUnitOfWorkAs(item));
        }

        protected override UnitOfWorkGroup MergeWith(UnitOfWorkItem item)
        {
            _items.Add(item);
            return this;
        }

        public override void CollectUnitsThatRequireFlush(ICollection<UnitOfWorkWrapper> units)
        {
            var itemsThatRequireFlush = _items.Where(item => item.RequiresFlush()).ToArray();
            if (itemsThatRequireFlush.Length == 1)
            {
                units.Add(itemsThatRequireFlush[0]);                
            }
            else if (itemsThatRequireFlush.Length == _items.Count)
            {
                units.Add(this);
            }
            else if (itemsThatRequireFlush.Length > 1)
            {
                units.Add(new UnitOfWorkGroup(itemsThatRequireFlush));
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
