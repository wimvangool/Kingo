using System;
using Syztem.Resources;

namespace Syztem.ComponentModel.FluentValidation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="Member{TValue}" />.
    /// </summary>
    public static class BooleanMemberExtensions
    {
        /// <summary>
        /// Verifies that the specified member is <c>true</c>.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="Member{TValue}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member "/> is <c>null</c>.
        /// </exception>
        public static void IsTrue(this Member<bool> member, FormattedString errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new FormattedString(ValidationMessages.BooleanMember_IsTrue_Failed, member);
            }
            member.Satisfies(value => value, errorMessage);
        }

        /// <summary>
        /// Verifies that the specified member is <c>false</c>.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="Member{TValue}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member "/> is <c>null</c>.
        /// </exception>
        public static void IsFalse(this Member<bool> member, FormattedString errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new FormattedString(ValidationMessages.BooleanMember_IsFalse_Failed, member);
            }
            member.Satisfies(value => !value, errorMessage);
        }
    }
}
