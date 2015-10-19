using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== IsNotInstanceOf ======]

        /// <summary>
        /// Verifies that this member's value is not an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TMessage, TValue> IsNotInstanceOf<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Type type, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that this member's value is not an instance of <paramref name="typeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="typeFactory">Delegate that returns the type to compare this member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="typeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TMessage, TValue> IsNotInstanceOf<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, Type> typeFactory, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== IsInstanceOf ======]

        /// <summary>
        /// Verifies that the member's value is an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="type">The type to compare the member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TMessage, TValue> IsInstanceOf<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Type type, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is an instance of <paramref name="typeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="typeFactory">Delegate that returns the type to compare the member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="typeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TMessage, TValue> IsInstanceOf<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, Type> typeFactory, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
