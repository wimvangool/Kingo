using System;

namespace Kingo
{
    /// <summary>
    /// Represents an event-argument of events that can be raised when the value of a property changes.
    /// </summary>
    /// <typeparam name="TValue">Property type.</typeparam>
    public class PropertyChangedEventArgs<TValue> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEventArgs{T}" /> class.
        /// </summary>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public PropertyChangedEventArgs(TValue oldValue, TValue newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// The old value of the property.
        /// </summary>
        public TValue OldValue
        {
            get;
        }

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public TValue NewValue
        {
            get;
        }
    }
}
