using System;
using System.Collections.Generic;

namespace Syztem
{
    internal static class Comparer
    {
        internal static bool IsEqualTo<TValue>(TValue value, IComparable<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return ReferenceEquals(value, null);
            }
            return other.CompareTo(value) == 0;
        }

        internal static bool IsSmallerThan<TValue>(TValue value, IComparable<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return other.CompareTo(value) > 0;
        }

        internal static bool IsSmallerThanOrEqualTo<TValue>(TValue value, IComparable<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return ReferenceEquals(value, null);
            }
            return other.CompareTo(value) >= 0;
        }

        internal static bool IsGreaterThan<TValue>(TValue value, IComparable<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return !ReferenceEquals(value, null);
            }
            return other.CompareTo(value) < 0;
        }

        internal static bool IsGreaterThanOrEqualTo<TValue>(TValue value, IComparable<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return true;
            }
            return other.CompareTo(value) <= 0;
        }

        internal static IComparer<TValue> EnsureComparer<TValue>(IComparer<TValue> comparer)
        {
            return comparer ?? Comparer<TValue>.Default;
        } 
    }
}
