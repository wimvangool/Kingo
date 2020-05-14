using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Kingo.Clocks;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{   
    internal sealed class MessageBus : IMessageBus
    {
        #region [====== State ======]

        private abstract class State
        {
            public abstract IReadOnlyList<Message<object>> Messages
            {
                get;
            }

            public abstract void Send(object command, TimeSpan delay);

            public abstract void Send(object command, DateTimeOffset? deliveryTime = null);

            public abstract void Publish(object @event, TimeSpan delay);

            public abstract void Publish(object @event, DateTimeOffset? deliveryTime = null);
        }

        #endregion

        #region [====== UncommittedState ======]

        private sealed class UncommittedState : State
        {
            private readonly MessageFactory _messageFactory;
            private readonly List<Message<object>> _messages;
            private readonly IClock _clock;

            public UncommittedState(MessageFactory messageFactory, IClock clock)
            {
                _messageFactory = messageFactory;
                _messages = new List<Message<object>>();
                _clock = clock;
            }

            public override IReadOnlyList<Message<object>> Messages =>
                _messages;

            public override void Send(object command, TimeSpan delay) =>
                Send(command, ToDeliveryTime(delay));

            public override void Send(object command, DateTimeOffset? deliveryTime = null) =>
                Add(IsNotNull(command, nameof(command)), MessageKind.Command, deliveryTime);

            public override void Publish(object @event, TimeSpan delay) =>
                Publish(@event, ToDeliveryTime(delay));

            public override void Publish(object @event, DateTimeOffset? deliveryTime = null) =>
                Add(IsNotNull(@event, nameof(@event)), MessageKind.Event, deliveryTime);

            private void Add(object content, MessageKind kind, DateTimeOffset? deliveryTime) =>
                _messages.Add(_messageFactory.CreateMessage(kind, MessageDirection.Output, MessageHeader.Unspecified, content, deliveryTime));

            private DateTimeOffset ToDeliveryTime(TimeSpan delay)
            {
                if (delay < TimeSpan.Zero)
                {
                    throw NewNegativeDelayNotAllowedException(delay);
                }
                return _clock.UtcDateAndTime().Add(delay);
            }

            private static Exception NewNegativeDelayNotAllowedException(in TimeSpan delay)
            {
                var messageFormat = ExceptionMessages.MessageBus_NegativeDelayNotAllowed;
                var message = string.Format(messageFormat, delay);
                return new ArgumentOutOfRangeException(nameof(delay), message);
            }
        }

        #endregion

        #region [====== CommittedState ======]

        private sealed class CommittedState : State
        {
            private readonly Message<object>[] _messages;

            public CommittedState()
            {
                _messages = new Message<object>[0];
            }

            public override IReadOnlyList<Message<object>> Messages =>
                _messages;

            public override void Send(object command, TimeSpan delay) =>
                throw NewMessageBusCommittedException();

            public override void Send(object command, DateTimeOffset? deliveryTime = null) =>
                throw NewMessageBusCommittedException();

            public override void Publish(object @event, TimeSpan delay) =>
                throw NewMessageBusCommittedException();

            public override void Publish(object @event, DateTimeOffset? deliveryTime = null) =>
                throw NewMessageBusCommittedException();

            private static Exception NewMessageBusCommittedException() =>
                new InvalidOperationException(ExceptionMessages.MessageBus_AlreadyCommitted);
        }

        #endregion

        private State _state;

        public MessageBus(MessageFactory messageFactory, IClock clock)
        {   
            _state = new UncommittedState(messageFactory, clock);
        }

        #region [====== IReadOnlyList<IMessage> ======]

        public int Count =>
            _state.Messages.Count;

        public IMessage this[int index] =>
            _state.Messages[index];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<IMessage> GetEnumerator() =>
            _state.Messages.GetEnumerator();

        /// <inheritdoc />
        public override string ToString() =>
            ToString(_state.Messages.Count(IsCommand), _state.Messages.Count(IsEvent));

        private static string ToString(int commandCount, int eventCount) =>
            $"{commandCount} command(s), {eventCount} event(s)";

        private static bool IsCommand(IMessage message) =>
            message.Kind == MessageKind.Command;

        private static bool IsEvent(IMessage message) =>
            message.Kind == MessageKind.Event;

        #endregion

        #region [====== Send & Publish ======]

        public void Send(object command, TimeSpan delay) =>
            _state.Send(command, delay);

        public void Send(object command, DateTimeOffset? deliveryTime = null) =>
            _state.Send(command, deliveryTime);

        public void Publish(object @event, TimeSpan delay) =>
            _state.Publish(@event, delay);

        public void Publish(object @event, DateTimeOffset? deliveryTime = null) =>
            _state.Publish(@event, deliveryTime);

        #endregion

        #region [====== CommitResult ======]

        public MessageHandlerOperationResult<TMessage> CommitResult<TMessage>(Message<TMessage> input) =>
            new MessageHandlerOperationResult<TMessage>(input, CommitOutput(), 1);

        private IEnumerable<Message<object>> CommitOutput() =>
            Interlocked.Exchange(ref _state, new CommittedState()).Messages;

        #endregion
    }
}
