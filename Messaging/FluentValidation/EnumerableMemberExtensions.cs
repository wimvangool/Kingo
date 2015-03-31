using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Globalization;
using System.Linq;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="Member{IEnumerable}" />.
    /// </summary>
    public static class EnumerableMemberExtensions
    {
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
        public static Member<TValue> HasElementAt<TValue>(this Member<IEnumerable<TValue>> member, int index, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.EnumerableMember_HasElementAt_Failed, member, index);
            }
            Func<IEnumerable<TValue>, bool> constraint = collection => 0 <= index && index < collection.Count();
            Func<IEnumerable<TValue>, TValue> selector = collection => collection.ElementAt(index);
            var newMemberName = string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", member.Name, index);

            return member.Satisfies(constraint, selector, newMemberName, errorMessage);
        }        

        #endregion
    }
}
