using System.Collections.Generic;

namespace System.ComponentModel.FluentValidation
{
    internal sealed class Equatable<TValue> : IEquatable<TValue>
    {
        private readonly TValue _value;
        private readonly IEqualityComparer<TValue> _comparer;        

        internal Equatable(TValue value, IEqualityComparer<TValue> comparer)
        {
            _value = value;
            _comparer = comparer;
        }

        public bool Equals(TValue other)
        {
            if (ReferenceEquals(_comparer, null))
            {
                return Equals(_value, other);
            }
            return _comparer.Equals(_value, other);
        }
    }
}
