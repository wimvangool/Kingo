using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Kingo.Resources;

namespace Kingo
{
    /// <summary>
    /// Contains some helper methods for types implementing the <see cref="IComparable{T}" /> interface.
    /// </summary>
    [Serializable]
    public abstract class Comparable : Equatable
    {
        private const int _Zero = 0;

        /// <summary>
        /// Value indicating that left is less than right.
        /// </summary>
        public const int Less = -1;

        /// <summary>
        /// Value indicating that left is equal to right.
        /// </summary>
        public const int Equal = _Zero;

        /// <summary>
        /// Value indicating that left is greater than right.
        /// </summary>
        public const int Greater = 1;

        /// <summary>
        /// Determines whether <paramref name="instance"/> is less than, equal to or greater than <paramref name="obj"/>.
        /// </summary>
        /// <typeparam name="T">Type of the objects to compare.</typeparam>
        /// <param name="instance">The typed instance.</param>
        /// <param name="obj">The untyped instance.</param>
        /// <returns>
        /// A negative value if <paramref name="instance"/> is less than <paramref name="obj"/>,
        /// zero if <paramref name="instance"/> is equal to <paramref name="obj"/>, or
        /// A positive value if <paramref name="instance"/> is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="obj"/> is not of type <typeparamref name="T"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareValues<T>(T instance, object obj) where T : struct, IComparable<T>
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (obj is T)
            {
                return instance.CompareTo((T) obj);
            }
            throw NewUnexpectedTypeException(typeof(T), obj.GetType());
        }

        /// <summary>
        /// Determines whether <paramref name="instance"/> is less than, equal to or greater than <paramref name="obj"/>.
        /// </summary>
        /// <typeparam name="T">Type of the objects to compare.</typeparam>
        /// <param name="instance">The typed instance.</param>
        /// <param name="obj">The untyped instance.</param>
        /// <returns>
        /// A negative value if <paramref name="instance"/> is less than <paramref name="obj"/>,
        /// zero if <paramref name="instance"/> is equal to <paramref name="obj"/>, or
        /// A positive value if <paramref name="instance"/> is greater than <paramref name="obj"/>.
        /// </returns>        
        /// <exception cref="ArgumentException">
        /// <paramref name="obj"/> is not of type <typeparamref name="T"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareReferences<T>(T instance, object obj) where T : class, IComparable<T>
        {
            if (ReferenceEquals(obj, null))
            {
                return Greater;
            }
            if (ReferenceEquals(obj, instance))
            {
                return Equal;
            }
            var other = obj as T;
            if (other == null)
            {
                throw NewUnexpectedTypeException(typeof(T), obj.GetType());
            }
            return instance.CompareTo(other);
        }

        /// <summary>
        /// Determines whether <paramref name="left"/> is less than <paramref name="right"/>.
        /// </summary>
        /// <typeparam name="T">Type of the objects to compare.</typeparam>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> is less than <paramref name="right"/>; otherwise <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThan<T>(T left, T right) where T : IComparable<T> =>
             Compare(left, right) < _Zero;

        /// <summary>
        /// Determines whether <paramref name="left"/> is less than or equal to <paramref name="right"/>.
        /// </summary>
        /// <typeparam name="T">Type of the objects to compare.</typeparam>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThanOrEqualTo<T>(T left, T right) where T : IComparable<T> =>
             Compare(left, right) <= _Zero;

        /// <summary>
        /// Determines whether <paramref name="left"/> is greater than <paramref name="right"/>.
        /// </summary>
        /// <typeparam name="T">Type of the objects to compare.</typeparam>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThan<T>(T left, T right) where T : IComparable<T> =>
             Compare(left, right) > _Zero;

        /// <summary>
        /// Determines whether <paramref name="left"/> is greater than or equal to <paramref name="right"/>.
        /// </summary>
        /// <typeparam name="T">Type of the objects to compare.</typeparam>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThanOrEqualTo<T>(T left, T right) where T : IComparable<T> =>
             Compare(left, right) >= _Zero;

        /// <summary>
        /// Determines whether <paramref name="left"/> is less than, equal to or greater than <paramref name="right"/>.
        /// </summary>
        /// <typeparam name="T">Type of the rightects to compare.</typeparam>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// A negative value if <paramref name="left"/> is less than <paramref name="right"/>,
        /// zero if <paramref name="left"/> is equal to <paramref name="right"/>, or
        /// a positive value if <paramref name="left"/> is greater than <paramref name="right"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare<T>(T left, T right) where T : IComparable<T>
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null) ? Equal : Less;
            }
            if (ReferenceEquals(right, null))
            {
                return Greater;
            }
            return left.CompareTo(right);
        }

        /// <summary>
        /// Creates and returns a new <see cref="ArgumentException" /> indicating that a certain instance could not be compared
        /// to another instance because their types didn't match.
        /// </summary>
        /// <param name="instanceType">Type of the main instance.</param>
        /// <param name="otherType">Type of the instance to compare.</param>
        /// <returns>A new <see cref="ArgumentException" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceType"/> or <paramref name="otherType"/> is <c>null</c>.
        /// </exception>
        public static Exception NewUnexpectedTypeException(Type instanceType, Type otherType)
        {
            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }
            if (otherType == null)
            {
                throw new ArgumentNullException(nameof(otherType));
            }
            var messageFormat = ExceptionMessages.Comparable_IncomparableType;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, otherType.GetType(), instanceType);
            return new ArgumentException(message, nameof(otherType));
        }
    }
}
