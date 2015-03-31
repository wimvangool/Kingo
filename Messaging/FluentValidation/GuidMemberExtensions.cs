using System;
using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Linq;
using System.Text;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="Member{TValue}" />.
    /// </summary>
    public static class GuidMemberExtensions
    {
        #region [====== IsNotEmpty ======]

        /// <summary>
        /// Verifies that the value is not an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<Guid> IsNotEmpty(this Member<Guid> member)
        {
            return IsNotEmpty(member, new ErrorMessage(ValidationMessages.Member_GuidIsNotEmpty_Failed, member));
        }

        /// <summary>
        /// Verifies that the value is not an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<Guid> IsNotEmpty(this Member<Guid> member, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEmpty, errorMessage);
        }

        private static bool IsNotEmpty(Guid value)
        {
            return !value.Equals(Guid.Empty);
        }

        #endregion

        #region [====== IsEmpty ======]

        /// <summary>
        /// Verifies that the value is an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<Guid> IsEmpty(this Member<Guid> member)
        {
            return IsEmpty(member, new ErrorMessage(ValidationMessages.Member_GuidIsEmpty_Failed, member));
        }

        /// <summary>
        /// Verifies that the value is an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<Guid> IsEmpty(this Member<Guid> member, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEmpty, errorMessage);
        }

        private static bool IsEmpty(Guid value)
        {
            return value.Equals(Guid.Empty);
        }

        #endregion
    }
}
