using System.Collections.Generic;

namespace System.ComponentModel.FluentValidation
{
    internal sealed class InternalRange<TValue> : IRange<TValue>
    {        
        private readonly Range<Comparable<TValue>> _range;
        private readonly IComparer<TValue> _comparer;

        internal InternalRange(TValue left, TValue right, RangeOptions options)
            : this(left, right, null, options) { }

        internal InternalRange(TValue left, TValue right, IComparer<TValue> comparer = null, RangeOptions options = RangeOptions.None)
        {
            var leftValue = new Comparable<TValue>(left, comparer);
            var rightValue = new Comparable<TValue>(right, comparer);

            _range = new Range<Comparable<TValue>>(leftValue, rightValue, options);
            _comparer = Comparer.EnsureComparer(comparer);
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
