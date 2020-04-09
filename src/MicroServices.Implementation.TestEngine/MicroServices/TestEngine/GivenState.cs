using System;
using static Kingo.MicroServices.TestEngine.MicroProcessorTestContext;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class GivenState : MicroProcessorTestState, IGivenState
    {
        #region [====== GivenTimeOrMessageState ======]

        private abstract class GivenTimeOrMessageState : MicroProcessorTestState
        {
            private readonly GivenState _givenState;

            protected GivenTimeOrMessageState(GivenState givenState)
            {
                _givenState = givenState;
            }

            protected override MicroProcessorTest Test =>
                _givenState._test;

            public override string ToString() =>
                _givenState.ToString();

            protected void AddOperation(MicroProcessorTestOperation operation) =>
                _givenState.AddGivenOperation(this, operation);
        }

        #endregion

        #region [====== TimeHasPassedForState ======]

        private sealed class TimeHasPassedForState : GivenTimeOrMessageState, IGivenTimeHasPassedForState
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

        private sealed class MessageState<TMessage> : GivenTimeOrMessageState, IGivenCommandOrEventState<TMessage>
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

            public void IsExecutedBy<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage> =>
                IsExecutedBy<TMessageHandler>(ConfigureMessage(message));

            public void IsExecutedBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TMessage> =>
                AddOperation(new CommandOperation<TMessage, TMessageHandler>(configurator));

            public void IsExecutedBy(IMessageHandler<TMessage> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
                AddOperation(new CommandOperation<TMessage>(messageHandler, configurator));

            public void IsHandledBy<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage> =>
                IsHandledBy<TMessageHandler>(ConfigureMessage(message));

            public void IsHandledBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TMessage> =>
                AddOperation(new EventOperation<TMessage, TMessageHandler>(configurator));

            public void IsHandledBy(IMessageHandler<TMessage> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
                AddOperation(new EventOperation<TMessage>(messageHandler, configurator));
        }

        #endregion

        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestTimeline _timeline;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        public GivenState(MicroProcessorTest test, MicroProcessorTestTimeline timeline, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _timeline = timeline;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Scheduling new operation... ({_givenOperations})";

        public void TimeIs(DateTimeOffset value) =>
            AddGivenOperation(this, _timeline.CreateTimeIsOperation(value));

        public IGivenTimeHasPassedForState TimeHasPassedFor(int value) =>
            _test.MoveToState(this, new TimeHasPassedForState(this, value));

        public void TimeHasPassed(TimeSpan value) =>
            AddGivenOperation(this, _timeline.CreateTimeHasPassedOperation(value));

        private void AddGivenOperation(MicroProcessorTestState expectedState, MicroProcessorTestOperation givenOperation) =>
            _test.MoveToState(expectedState, new ReadyToConfigureTestState(_test, _timeline, _givenOperations.Enqueue(givenOperation)));

        public IGivenCommandOrEventState<TMessage> Message<TMessage>() =>
            _test.MoveToState(this, new MessageState<TMessage>(this));
    }
}
