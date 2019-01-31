using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a set of published events.
    /// </summary>
    public class EventStream : ReadOnlyList<object>
    {
        /// <summary>
        /// Represents an empty event stream.
        /// </summary>
        public static readonly EventStream Empty = new EventStream(Enumerable.Empty<object>());

        private readonly object[] _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStream" /> class.
        /// </summary>
        /// <param name="events">The events of this stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        public EventStream(IEnumerable<object> events)
        {
            _events = events.WhereNotNull().ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStream" /> class.
        /// </summary>
        /// <param name="stream">The stream to copy.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        protected EventStream(EventStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            _events = stream._events;
        }

        #region [====== ReadOnlyList ======]

        /// <inheritdoc />
        public override object this[int index] =>
            _events[index];

        /// <inheritdoc />
        public override int Count =>
            _events.Length;

        /// <inheritdoc />
        public override IEnumerator<object> GetEnumerator() =>
            (IEnumerator<object>) _events.GetEnumerator();

        #endregion

        #region [====== AssertEvent & GetEvent ======]

        /// <summary>
        /// Asserts that this stream contains an event at the specified <paramref name="index"/>
        /// that is of the specified type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">Expected type of the event.</typeparam>
        /// <param name="index">Index of the event.</param>
        /// <param name="assertion">Optional delegate to verify the details of the event.</param>        
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// There is no event at the specified <paramref name="index"/>, or the event at that
        /// <paramref name="index"/> is not of type <typeparamref name="TEvent"/>.
        /// </exception>
        public void AssertEvent<TEvent>(int index, Action<TEvent> assertion = null)
        {
            object @event;

            try
            {
                @event = _events[index];
            }
            catch (IndexOutOfRangeException exception)
            {
                if (index < 0)
                {
                    throw;
                }
                throw NewEventNotFoundException(typeof(TEvent), index, Count, exception);
            }
            if (@event is TEvent expectedEvent)
            {
                assertion?.Invoke(expectedEvent);                
            }
            else
            {
                throw NewEventOfNotOfExpectedTypeException(typeof(TEvent), index, @event.GetType());
            }            
        }

        private static Exception NewEventNotFoundException(Type expectedType, int index, int eventCount, Exception innerException)
        {
            var messageFormat = ExceptionMessages.EventStream_EventNotFound;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), index, eventCount);
            return new TestFailedException(message, innerException);
        }

        private static Exception NewEventOfNotOfExpectedTypeException(Type expectedType, int index, Type actualType)
        {
            var messageFormat = ExceptionMessages.EventStream_EventNotOfExpectedType;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), index, actualType.FriendlyName());
            return new TestFailedException(message);
        }        

        /// <summary>
        /// Returns the event at the specified <paramref name="index" />.
        /// </summary>
        /// <typeparam name="TEvent">Expected type of the event.</typeparam>
        /// <param name="index">Index of the requested event.</param>
        /// <returns>The event at index <paramref name="index"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is not a valid index for this stream.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The event at the specified <paramref name="index"/> is not of type <typeparamref name="TEvent"/>.
        /// </exception>
        protected TEvent GetEvent<TEvent>(int index) =>
            (TEvent) _events[index];

        #endregion
    }
}
