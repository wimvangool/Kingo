using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Globalization;
using System.Linq;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="Member{TValue}" />.
    /// </summary>
    public static class CollectionMemberExtensions
    {
        #region [====== IsNotNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection has at least one element.
        /// </summary>
        /// <typeparam name="TValue">Type of the values in the collection.</typeparam>
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="Member{TValue}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member "/> is <c>null</c>.
        /// </exception>
        public static Member<ICollection<TValue>> IsNotNullOrEmpty<TValue>(this Member<ICollection<TValue>> member, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.Member_CollectionIsNotNullOrEmpty_Failed, member);
            }
            return member.Satisfies(collection => collection != null && collection.Count > 0, errorMessage);
        }

        #endregion

        #region [====== ElementAt ======]

        /// <summary>
        /// Verifies that the specified collection contains a value at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the values in the collection.</typeparam>
        /// <param name="member">A member.</param>
        /// <param name="index">The index to verify.</param>   
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="Member{TValue}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member "/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> ElementAt<TValue>(this Member<ICollection<TValue>> member, int index, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.Member_CollectionElementAt_Failed, member, index);
            }
            Func<ICollection<TValue>, bool> constraint = collection => collection != null && 0 <= index && index < collection.Count;
            Func<ICollection<TValue>, TValue> selector = collection => collection.ElementAt(index);
            var newMemberName = string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", member.Name, index);

            return member.Satisfies(constraint, selector, newMemberName, errorMessage);
        }

        #endregion
    }
}
