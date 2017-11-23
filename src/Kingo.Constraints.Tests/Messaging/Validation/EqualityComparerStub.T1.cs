﻿using System.Collections.Generic;

namespace Kingo.Messaging.Validation
{
    internal sealed class EqualityComparerStub<T> : IEqualityComparer<T>
    {
        private readonly bool _result;

        internal EqualityComparerStub(bool result)
        {
            _result = result;
        }

        public bool Equals(T x, T y) =>
             _result;

        public int GetHashCode(T obj) =>
             0;
    }
}
