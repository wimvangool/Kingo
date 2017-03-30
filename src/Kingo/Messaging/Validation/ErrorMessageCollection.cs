using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IErrorMessageCollection" /> class.
    /// </summary>
    public abstract class ErrorMessageCollection : IErrorMessageCollection
    {
        /// <summary>
        /// Returns the <see cref="IFormatProvider"/> that is used to format incoming error messages.
        /// </summary>
        protected virtual IFormatProvider FormatProvider =>
            null;

        /// <inheritdoc />
        public void Add(IErrorMessageBuilder errorMessage, string memberName) =>
            Add(errorMessage, memberName, ErrorInheritanceLevel.NotInherited);

        /// <inheritdoc />
        public void Add(IErrorMessageBuilder errorMessage, string memberName, ErrorInheritanceLevel inheritanceLevel)
        {
            if (errorMessage == null)
            {
                return;
            }
            Add(errorMessage.ToString(FormatProvider), memberName, inheritanceLevel);
        }

        /// <inheritdoc />
        public void Add(string errorMessage, string memberName) =>
            Add(errorMessage, memberName, ErrorInheritanceLevel.NotInherited);

        /// <inheritdoc />
        public abstract void Add(string errorMessage, string memberName, ErrorInheritanceLevel inheritanceLevel);                       
    }
}
