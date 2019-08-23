using System;
using System.Runtime.CompilerServices;

namespace Kingo
{
    /// <summary>
    /// Contains some helper methods for types implementing the <see cref="IEquatable{T}" /> interface.
    /// </summary>
    [Serializable]
    public abstract class Equatable
    {        
        /// <summary>
        /// Determines whether <paramref name="left"/> is equal to <paramref name="right"/>.
        /// </summary>
        /// <typeparam name="T">Type of the instances to compare.</typeparam>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns><c>true</c> if both instances are equal; otherwise <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals<T>(T left, T right) where T : class, IEquatable<T>
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);            
        }
    }
}
