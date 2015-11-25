using System;

namespace Kingo.Constraints
{
    internal sealed class EquatableObject<TValue> : IEquatable<TValue>
    {
        private readonly object _value;

        internal EquatableObject(object value)
        {
            _value = value;
        }

        public bool Equals(TValue other)
        {
            return Equals(_value, other);
        }

        public override string ToString()
        {
            return ReferenceEquals(_value, null) ? StringTemplate.NullValue : _value.ToString();
        }
    }
}
