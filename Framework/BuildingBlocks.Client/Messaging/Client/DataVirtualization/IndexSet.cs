using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingo.BuildingBlocks.Messaging.Client.DataVirtualization
{
    internal sealed class IndexSet
    {
        private readonly LinkedList<IndexRange> _ranges;

        public IndexSet()
        {
            _ranges = new LinkedList<IndexRange>();
        }

        public int Count
        {
            get
            {
                lock (_ranges)
                {
                    return _ranges.Sum(range => range.Count);
                }
            }
        }

        public void Add(int index)
        {             
            if (index < 0)
            {
                throw IndexRange.NewIndexOutOfRangeException(index);
            }
            lock (_ranges)
            {
                var node = _ranges.First;
                if (node != null)
                {
                    do
                    {
                        if (TryAdd(index, node))
                        {
                            return;
                        }
                    }
                    while ((node = node.Next) != null);
                }
                _ranges.AddLast(new IndexRange(index));
            }
        }

        private bool TryAdd(int index, LinkedListNode<IndexRange> node)
        {
            var range = node.Value;

            if (index < range.MinValue - 1)
            {
                _ranges.AddBefore(node, new IndexRange(index));

                return true;
            }
            if (range.IsLeftAdjacentTo(index))
            {
                node.Value = range.AddToLeft();

                return true;
            }
            if (range.Contains(index))
            {
                return true;
            }
            if (range.IsRightAdjacentTo(index))
            {
                node.Value = range = range.AddToRight();

                // By adding one index to the node, the range may now be directly adjacent
                // to the next range. In that case, we must merge the two nodes, which is
                // done by extending the range with all values of the next one and then
                // removing the next node.
                if (node.Next != null && range.IsRightAdjacentTo(node.Next.Value))
                {
                    node.Value = range.AddToRight(node.Next.Value.Count);

                    _ranges.Remove(node.Next);
                }
                return true;
            }
            return false;
        }              

        public void Remove(int index)
        {
            if (index < 0)
            {
                return;
            }
            lock (_ranges)
            {
                var node = _ranges.First;
                if (node == null)
                {
                    return;
                }
                do
                {
                    if (TryRemove(index, node))
                    {
                        return;
                    }
                }
                while ((node = node.Next) != null);
            }
        }

        private bool TryRemove(int index, LinkedListNode<IndexRange> node)
        {
            var range = node.Value;

            if (range.Count == 1 && range.Contains(index))
            {
                _ranges.Remove(node);

                return true;
            }
            if (index == range.MinValue)
            {
                node.Value = range.RemoveFromLeft();

                return true;
            }
            if (index == range.MaxValue)
            {
                node.Value = range.RemoveFromRight();

                return true;
            }
            if (range.Contains(index))
            {
                // If the index falls in the middle of an existing range, we have to split up
                // the existing range by a left (lower) and right (higher) range.
                var leftRange = new IndexRange(range.MinValue, index - range.MinValue);
                var rightRange = new IndexRange(index + 1, range.MaxValue - index);

                node.Value = leftRange;

                _ranges.AddAfter(node, rightRange);

                return true;
            }
            return false;
        } 
       
        public void Clear()
        {
            lock (_ranges)
            {
                _ranges.Clear();    
            }            
        }

        public bool Contains(int index)
        {
            lock (_ranges)
            {
                return _ranges.Any(range => range.Contains(index));
            }
        }

        public override string ToString()
        {
            lock (_ranges)
            {
                var set = new StringBuilder(_ranges.Count * 4);

                foreach (var range in _ranges)
                {
                    set.Append(range);
                }
                return set.ToString();
            }
        }
    }
}
