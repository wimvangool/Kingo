using System;

namespace Kingo.Serialization
{
    /// <summary>
    /// When implemented by a class, represents a component that can convert a <see cref="string"/>
    /// to a byte-array and vice versa.
    /// </summary>
    public interface ITextEncoder
    {
        /// <summary>
        /// Converts the specified <paramref name="content"/> to a byte-array.
        /// </summary>
        /// <param name="content">The content to convert.</param>
        /// <returns>An array of bytes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="content"/> couldn't be encoded with this encoder.
        /// </exception>
        byte[] Encode(string content);

        /// <summary>
        /// Converts the specified <paramref name="content"/> to a string.
        /// </summary>
        /// <param name="content">The content to convert.</param>
        /// <returns>A string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="content"/> couldn't be decoded with this encoder.
        /// </exception>
        string Decode(byte[] content);
    }
}
