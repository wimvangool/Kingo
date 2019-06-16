using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{    
    internal sealed class EventBuffer : IEventBuffer, IReadOnlyList<IMessage>
    {
        public static readonly EventBuffer Empty = new EventBuffer(Enumerable.Empty<IMessage>());

        private readonly Event[] _events;

        public EventBuffer(IEnumerable<IMessage> messages) :
            this(messages.Where(message => message.Kind == MessageKind.Event).Select(message => CreateEvent(message.Instance))) { }

        private EventBuffer(IEnumerable<Event> events)
        {
            _events = events.ToArray();
        }        

        #region [====== IReadOnlyList<object> ======]

        public int Count =>
            _events.Length;

        public IMessage this[int index] =>
            _events[index];

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IMessage> GetEnumerator() =>
            _events.AsEnumerable().GetEnumerator();

        public override string ToString() =>
            EventBus.ToString(this);

        #endregion

        #region [====== IEventStream ======]

        public EventBuffer Append(EventBuffer buffer) =>
            new EventBuffer(_events.Concat(buffer._events));

        public async Task<MessageHandlerOperationResult> HandleWith(IMessageProcessor processor, MessageHandlerOperationContext context)
        {
            var result = EventBufferResult.Empty;

            foreach (var message in _events)
            {
                result = result.Append(await message.HandleWith(processor, context).ConfigureAwait(false));
            }
            return result;
        }

        #endregion

        #region [====== CreateEvent ======]

        private static readonly ConcurrentDictionary<Type, Func<object, Event>> _EventConstructors = new ConcurrentDictionary<Type, Func<object, Event>>();

        private static Event CreateEvent(object message) =>
            GetOrAddEventConstructorFor(message.GetType()).Invoke(message);

        private static Func<object, Event> GetOrAddEventConstructorFor(Type messageType) =>
            _EventConstructors.GetOrAdd(messageType, GetEventConstructorFor);

        private static Func<object, Event> GetEventConstructorFor(Type messageType)
        {
            var eventTypeDefinition = typeof(Event<>);
            var eventType = eventTypeDefinition.MakeGenericType(messageType);

            var constructor = eventType.GetConstructor(new[] { messageType });
            var messageParameter = Expression.Parameter(typeof(object), "event");
            var message = Expression.Convert(messageParameter, messageType);
            var body = Expression.New(constructor, message);

            return Expression.Lambda<Func<object, Event>>(body, messageParameter).Compile();
        }

        #endregion
    }
}
