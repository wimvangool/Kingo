using System.Collections.Generic;

namespace ServiceComponents.ComponentModel.Constraints
{
    internal sealed class RangeAdapter<TValue> : IRange<TValue>
    {        
        private readonly Range<Comparable<TValue>> _range;
        private readonly IComparer<TValue> _comparer;

        internal RangeAdapter(TValue left, TValue right, RangeOptions options)
            : this(left, right, null, options) { }

        internal RangeAdapter(TValue left, TValue right, IComparer<TValue> comparer = null, RangeOptions options = RangeOptions.None)
        {
            var leftValue = new Comparable<TValue>(left, comparer);
            var rightValue = new Comparable<TValue>(right, comparer);

            _range = new Range<Comparable<TValue>>(leftValue, rightValue, options);
            _comparer = Comparer.EnsureComparer(comparer);
        }

        public TValue Left
        {
            get { return _range.Left.Value; }
        }

        public TValue Right
        {
            get { return _range.Right.Value; }
        }

        public bool IsLeftInclusive
        {
            get { return _range.IsLeftInclusive; }
        }

        public bool IsRightInclusive
        {
            get { return _range.IsRightInclusive; }
        }

        public bool Contains(TValue value)
        {
            return _range.Contains(new Comparable<TValue>(value, _comparer));
        }

        public override string ToString()
        {
            return _range.ToString();
        }        
    }
}
