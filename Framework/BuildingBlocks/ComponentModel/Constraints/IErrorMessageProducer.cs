using System;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a producer of error messages.
    /// </summary>
    public interface IErrorMessageProducer
    {
        /// <summary>
        /// If <paramref name="consumer"/> is not <c>null</c>, produces a set of error messages
        /// and adds them to the specified <paramref name="consumer"/>.
        /// </summary>
        /// <param name="consumer">A consumer of error messages.</param>   
        /// <param name="formatProvider">A <see cref="IFormatProvider" /> that is used for placeholders that define a specific format.</param>
        /// <returns><c>true</c> if any error messages were added to the specified <paramref name="consumer"/>; otherwise <c>false</c>.</returns>    
        bool AddErrorMessagesTo(IErrorMessageConsumer consumer, IFormatProvider formatProvider = null);
    }
}
