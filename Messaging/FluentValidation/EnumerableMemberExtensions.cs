using System;
using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Globalization;
using System.Linq;
using System.Text;

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
        /// <returns>A <see cref="Member{TValue}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member "/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasElementAt<TValue>(this Member<IEnumerable<TValue>> member, int index)
        {
            return HasElementAt(member, index, new ErrorMessage(ValidationMessages.EnumerableMember_HasElementAt_Failed, member, index));
        }

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
        /// <paramref name="member "/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasElementAt<TValue>(this Member<IEnumerable<TValue>> member, int index, string errorMessage)
        {
            return HasElementAt(member, index, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the specified collection contains a value at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the values in the collection.</typeparam>
        /// <param name="member">A member.</param>
        /// <param name="index">The index to verify.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>A <see cref="Member{TValue}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member "/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasElementAt<TValue>(this Member<IEnumerable<TValue>> member, int index, string errorMessageFormat, object arg0)
        {
            return HasElementAt(member, index, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that the specified collection contains a value at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the values in the collection.</typeparam>
        /// <param name="member">A member.</param>
        /// <param name="index">The index to verify.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>A <see cref="Member{TValue}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member "/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasElementAt<TValue>(this Member<IEnumerable<TValue>> member, int index, string errorMessageFormat, object arg0, object arg1)
        {
            return HasElementAt(member, index, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that the specified collection contains a value at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the values in the collection.</typeparam>
        /// <param name="member">A member.</param>
        /// <param name="index">The index to verify.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>A <see cref="Member{TValue}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member "/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasElementAt<TValue>(this Member<IEnumerable<TValue>> member, int index, string errorMessageFormat, params object[] arguments)
        {
            return HasElementAt(member, index, new ErrorMessage(errorMessageFormat, arguments));
        }

        private static Member<TValue> HasElementAt<TValue>(Member<IEnumerable<TValue>> member, int index, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            Func<IEnumerable<TValue>, bool> constraint = collection => 0 <= index && index < collection.Count();
            Func<IEnumerable<TValue>, TValue> selector = collection => collection.ElementAt(index);
            var newMemberName = string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", member.Name, index);

            return member.Satisfies(constraint, errorMessage, selector, newMemberName);
        }        

        #endregion
    }
}
