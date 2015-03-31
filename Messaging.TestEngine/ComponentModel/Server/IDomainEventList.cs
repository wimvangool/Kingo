using System.ComponentModel.FluentValidation;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a list of events that were published by executing a <see cref="Scenario{TMessage}" />.
    /// </summary>
    public interface IDomainEventList
    {
        /// <summary>
        /// Returns the event at the specified <paramref name="index"/> in the form of a <see cref="Member{TValue}"/>".
        /// </summary>
        /// <param name="index">The index of the event.</param>
        /// <returns>The event at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is out of range of valid values.
        /// </exception>
        Member<object> this[int index]
        {
            get;
        }
    }
}
