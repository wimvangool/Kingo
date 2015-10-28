using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== Length ======]

        /// <summary>
        /// Returns the length of a string.
        /// </summary>
        /// <param name="member">A member.</param>                
        /// <returns>The specified <paramref name="member"/>'s length.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>        
        public static IMemberConstraint<TMessage, int> Length<TMessage>(this IMemberConstraint<TMessage, string> member)
        {
            return member.Select(value => value.Length);
        }

        #endregion
    }
}
