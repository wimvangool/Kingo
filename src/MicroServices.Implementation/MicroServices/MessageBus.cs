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

            public abstract void Add(Message<object> message);

            public abstract State Commit();

            /// <inheritdoc />
            public override string ToString() =>
                ToString(Messages.Count(IsCommand), Messages.Count(IsEvent));

            private static string ToString(int commandCount, int eventCount) =>
                $"{commandCount} command(s), {eventCount} event(s)";

            private static bool IsCommand(IMessage message) =>
                message.Kind == MessageKind.Command;

            private static bool IsEvent(IMessage message) =>
                message.Kind == MessageKind.Event;
        }

        #endregion

        #region [====== UncommittedState ======]

        private class UncommittedState : State
        {
            private readonly List<Message<object>> _messages;
            private readonly MessageBus _messageBus;

            private readonly MessageFactory _messageFactory;
            private readonly IClock _clock;

            public UncommittedState(MessageFactory messageFactory, IClock clock, MessageBus messageBus)
            {
                _messages = new List<Message<object>>();
                _messageBus = messageBus;
                _messageFactory = messageFactory;
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
                Add(_messageFactory.CreateMessage(kind, MessageDirection.Output, MessageHeader.Unspecified, content, deliveryTime));

            public override void Add(Message<object> message) =>
                _messages.Add(message);

            public override State Commit()
            {
                if (_messageBus != null)
                {
                    foreach (var message in _messages)
                    {
                        _messageBus._state.Add(message);
                    }
                }
                return new CommittedState();
            }

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

            public override void Add(Message<object> message) =>
                throw NewMessageBusCommittedException();

            public override State Commit() =>
                throw NewMessageBusCommittedException();

            public override string ToString() =>
                base.ToString() + " (Committed)";

            private static Exception NewMessageBusCommittedException() =>
                new InvalidOperationException(ExceptionMessages.MessageBus_AlreadyCommitted);
        }

        #endregion

        private State _state;

        public MessageBus(MessageFactory messageFactory, IClock clock, MessageBus messageBus = null)
        {
            _state = new UncommittedState(messageFactory, clock, messageBus);
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
            _state.ToString();

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
            new MessageHandlerOperationResult<TMessage>(input, CommitOutput());

        private IEnumerable<Message<object>> CommitOutput() =>
            Interlocked.Exchange(ref _state, _state.Commit()).Messages;

        #endregion
    }
}
