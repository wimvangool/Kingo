using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a builder of an error message.
    /// </summary>
    public interface IErrorMessageBuilder
    {                
        /// <summary>
        /// Assigns an argument to this error message that will be used to format the message on a call to one of the
        /// <see cref="ToString(IFormatProvider)" /> overloads. If an argument with the same name was already set, it
        /// will be replaced by the specified <paramref name="argument"/>.
        /// </summary>
        /// <param name="name">Name of the argument.</param>
        /// <param name="argument">Value of the argument.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name" /> is not a valid identifier, or
        /// an argument with the same name has already been added to this error message.
        /// </exception>
        void Put(string name, object argument);

        /// <summary>
        /// Adds an argument to this error message that will be used to format the message on a call to one of the
        /// <see cref="ToString(IFormatProvider)" /> overloads. If an argument with the same name was already set, it
        /// will be replaced by the specified <paramref name="argument"/>.
        /// </summary>
        /// <param name="name">Name of the argument.</param>
        /// <param name="argument">Value of the argument.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An argument with the same name has already been added to this error message.
        /// </exception>
        void Put(Identifier name, object argument);

        /// <summary>
        /// Returns a string-representation of the error message formatted using the specified <paramref name="formatProvider"/>.
        /// </summary>
        /// <param name="formatProvider">
        /// The <see cref="IFormatProvider" /> to use to format the error message.
        /// If <c>null</c>, the default formatter is used.
        /// </param>
        /// <returns>A string-representation of the error message.</returns>
        string ToString(IFormatProvider formatProvider);
    }
}
