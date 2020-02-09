using System;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class GivenState : MicroProcessorTestState, IGivenState
    {
        #region [====== GivenChildState ======]

        private abstract class GivenChildState : MicroProcessorTestState
        {
            private readonly GivenState _givenState;

            protected GivenChildState(GivenState givenState)
            {
                _givenState = givenState;
            }

            protected override MicroProcessorTest Test =>
                _givenState._test;

            public override string ToString() =>
                _givenState.ToString();

            protected void AddOperation(GivenOperation operation) =>
                _givenState.AddOperation(this, operation);
        }

        #endregion

        #region [====== TimeHasPassedForState ======]

        private sealed class TimeHasPassedForState : GivenChildState, IGivenTimeHasPassedForState
        {
            private readonly MicroProcessorTestTimeline _timeline;
            private readonly int _value;

            public TimeHasPassedForState(GivenState givenState, int value) : base(givenState)
            {
                if (value < 0)
                {
                    throw MicroProcessorTestTimeline.NewNegativeTimeUnitException(value);
                }
                _timeline = givenState._timeline;
                _value = value;
            }

            public void Days() =>
                AddOperation(TimeSpan.FromDays(_value));

            public void Hours() =>
                AddOperation(TimeSpan.FromHours(_value));

            public void Minutes() =>
                AddOperation(TimeSpan.FromMinutes(_value));

            public void Seconds() =>
                AddOperation(TimeSpan.FromSeconds(_value));

            public void Milliseconds() =>
                AddOperation(TimeSpan.FromMilliseconds(_value));

            public void Ticks() =>
                AddOperation(TimeSpan.FromTicks(_value));

            private void AddOperation(TimeSpan value) =>
                AddOperation(_timeline.CreateTimeHasPassedOperation(value));
        }

        #endregion

        #region [====== MessageState<TMessage> ======]

        private sealed class MessageState<TMessage> : GivenChildState, IGivenMessageState<TMessage>
        {
            public MessageState(GivenState givenState) : base(givenState)
            {
                // Whenever a new message-operation is scheduled, the timeline must commit to
                // absolute or relative time, because this can no longer change after a message-operation
                // has been executed.
                // For example, we don't want to run the first operation in relative time and then allow
                // a specific time (absolute time) to be set before the second operation runs, since that
                // can result in weird behavior (i.e. going backwards in time).
                givenState._timeline.CommitToAbsoluteOrRelativeTime();
            }

            public void IsExecutedBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TMessage> =>
                AddOperation(new GivenCommandOperation<TMessage, TMessageHandler>(configurator));

            public void IsExecutedByCommandHandler(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, IMessageHandler<TMessage> messageHandler) =>
                AddOperation(new GivenCommandOperation<TMessage>(configurator, messageHandler));

            public void IsHandledBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TMessage> =>
                AddOperation(new GivenEventOperation<TMessage, TMessageHandler>(configurator));

            public void IsHandledByEventHandler(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, IMessageHandler<TMessage> messageHandler) =>
                AddOperation(new GivenEventOperation<TMessage>(configurator, messageHandler));
        }

        #endregion

        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestTimeline _timeline;
        private readonly GivenOperationQueue _operations;

        public GivenState(MicroProcessorTest test, MicroProcessorTestTimeline timeline, GivenOperationQueue operations)
        {
            _test = test;
            _timeline = timeline;
            _operations = operations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Scheduling new operation... ({_operations})";

        public void TimeIs(DateTimeOffset value) =>
            AddOperation(this, _timeline.CreateTimeIsOperation(value));

        public IGivenTimeHasPassedForState TimeHasPassedFor(int value) =>
            _test.MoveToState(this, new TimeHasPassedForState(this, value));

        public void TimeHasPassed(TimeSpan value) =>
            AddOperation(this, _timeline.CreateTimeHasPassedOperation(value));

        private void AddOperation(MicroProcessorTestState expectedState, GivenOperation operation) =>
            _test.MoveToState(expectedState, new ReadyState(_test, _timeline, _operations.Enqueue(operation)));

        public IGivenMessageState<TMessage> Message<TMessage>() =>
            _test.MoveToState(this, new MessageState<TMessage>(this));
    }
}
