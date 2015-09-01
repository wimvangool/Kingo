using System;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// Contains specific methods for object to calculate a hashcode based on their (immutable) members.
    /// </summary>
    public static class HashCode
    {
        #region [====== GetHashCode ======]

        /// <summary>
        /// Returns the hashcode of the specified instance, or <c>0</c> if <paramref name="a"/> is <c>null</c>.
        /// </summary>
        /// <param name="a">The instance to get the hashcode for.</param>
        /// <returns>The hashcode of the specified instance.</returns>
        public static int Of(object a)
        {
            return a == null ? 0 : a.GetHashCode();
        }

        /// <summary>
        /// Returns the combined hashcode of the specified instances using a bitwise XOR operation.
        /// </summary>
        /// <param name="a">A certain instance.</param>
        /// <param name="b">Another instance.</param>
        /// <returns>A combined hashcode of the specified instances.</returns>
        public static int Of(object a, object b)
        {
            return Of(a) ^ Of(b);
        }

        /// <summary>
        /// Returns the combined hashcode of the specified instances using a bitwise XOR operation.
        /// </summary>
        /// <param name="a">A certain instance.</param>
        /// <param name="b">Another instance.</param>
        /// <param name="c">Yet another instance.</param>
        /// <returns>A combined hashcode of the specified instances.</returns>
        public static int Of(object a, object b, object c)
        {
            return Of(a) ^ Of(b) ^ Of(c);
        }

        /// <summary>
        /// Returns the combined hashcode of the specified instances using a bitwise XOR operation.
        /// </summary>
        /// <param name="instances">A collection of instances.</param>
        /// <returns>A combined hashcode of the specified instances.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instances"/> is <c>null</c>.
        /// </exception>
        public static int Of(params object[] instances)
        {
            if (instances == null)
            {
                throw new ArgumentNullException("instances");
            }
            var hashCode = 0;

            for (int index = 0; index < instances.Length; index++)
            {
                hashCode ^= Of(instances[index]);
            }
            return hashCode;
        }

        #endregion
    }
}
