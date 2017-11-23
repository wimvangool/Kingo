using System;

namespace Kingo.Messaging.Validation
{
    internal sealed class EquatableObject<TValue> : IEquatable<TValue>
    {
        private readonly object _value;

        internal EquatableObject(object value)
        {
            _value = value;
        }

        public bool Equals(TValue other) =>
             Equals(_value, other);

        public override string ToString() =>
             ReferenceEquals(_value, null) ? StringTemplate.NullValue : _value.ToString();
    }
}
