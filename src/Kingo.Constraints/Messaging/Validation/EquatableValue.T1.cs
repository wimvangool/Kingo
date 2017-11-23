﻿using System;
using System.Collections.Generic;

namespace Kingo.Messaging.Validation
{
    internal sealed class EquatableValue<TValue> : IEquatable<TValue>
    {
        private readonly TValue _value;
        private readonly IEqualityComparer<TValue> _comparer;        

        internal EquatableValue(TValue value, IEqualityComparer<TValue> comparer)
        {
            _value = value;
            _comparer = comparer;
        }

        public bool Equals(TValue other)
        {
            if (ReferenceEquals(_comparer, null))
            {
                if (ReferenceEquals(_value, null))
                {
                    return ReferenceEquals(other, null);
                }
                return _value.Equals(other);
            }
            return _comparer.Equals(_value, other);
        }

        public override string ToString() => _value.ToString();
    }
}
