using System;
using System.Threading;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class MicroProcessorTestTimeline
    {
        #region [====== State ======]

        private abstract class State
        {
            protected State(MicroProcessorTestTimeline timeline)
            {
                Timeline = timeline;
            }

            protected MicroProcessorTestTimeline Timeline
            {
                get;
            }

            public virtual void CommitToAbsoluteOrRelativeTime() { }

            public abstract State TimeIs(DateTimeOffset value);

            public abstract State TimeHasPassed(TimeSpan value);

            public abstract MicroProcessorTestOperation CreateGivenOperation();

            protected State MoveToState(State newState) =>
                Timeline.MoveToState(this, newState);
        }

        #endregion

        #region [====== DefaultState ======]

        private sealed class DefaultState : State
        {
            public DefaultState(MicroProcessorTestTimeline timeline) :
                base(timeline) { }

            public override void CommitToAbsoluteOrRelativeTime() =>
                MoveToState(new RelativeTimeState(Timeline));

            public override State TimeIs(DateTimeOffset value) =>
                MoveToState(new AbsoluteTimeState(Timeline, value));

            public override State TimeHasPassed(TimeSpan value) =>
                MoveToState(new RelativeShiftedTimeState(Timeline, value));

            public override MicroProcessorTestOperation CreateGivenOperation() =>
                new NullOperation();
        }

        #endregion

        #region [====== AbsoluteTimeState ======]

        private sealed class AbsoluteTimeState : State
        {
            private readonly DateTimeOffset _valueUtc;

            public AbsoluteTimeState(MicroProcessorTestTimeline timeline, DateTimeOffset value) :
                base(timeline)
            {
                _valueUtc = value.ToUniversalTime();
            }

            public override State TimeIs(DateTimeOffset value)
            {
                var newState = new AbsoluteTimeState(Timeline, value);
                if (newState._valueUtc <= _valueUtc)
                {
                    throw NewInvalidTimeException(_valueUtc, value);
                }
                return Timeline.MoveToState(this, newState);
            }

            public override State TimeHasPassed(TimeSpan value) =>
                MoveToState(new AbsoluteTimeState(Timeline, _valueUtc.Add(value)));

            public override MicroProcessorTestOperation CreateGivenOperation() =>
                new TimeIsOperation(_valueUtc);
        }

        #endregion

        #region [====== RelativeTimeState ======]

        private class RelativeTimeState : State
        {
            public RelativeTimeState(MicroProcessorTestTimeline timeline) :
                base(timeline) { }

            public override State TimeIs(DateTimeOffset value) =>
                throw NewSpecificTimeNotAllowedException(value);

            public override State TimeHasPassed(TimeSpan value) =>
                MoveToState(new RelativeShiftedTimeState(Timeline, value));

            public override MicroProcessorTestOperation CreateGivenOperation() =>
                new NullOperation();
        }

        #endregion

        #region [====== RelativeShiftedTimeState ======]

        private sealed class RelativeShiftedTimeState : RelativeTimeState
        {
            private readonly TimeSpan _value;

            public RelativeShiftedTimeState(MicroProcessorTestTimeline timeline, TimeSpan value) :
                base(timeline)
            {
                _value = value;
            }

            public override State TimeHasPassed(TimeSpan value) =>
                MoveToState(new RelativeShiftedTimeState(Timeline, _value.Add(value)));

            public override MicroProcessorTestOperation CreateGivenOperation() =>
                new TimeHasPassedOperation(_value);
        }

        #endregion

        private State _state;

        public MicroProcessorTestTimeline()
        {
            _state = new DefaultState(this);
        }

        public void CommitToAbsoluteOrRelativeTime() =>
            _state.CommitToAbsoluteOrRelativeTime();

        public MicroProcessorTestOperation CreateTimeIsOperation(DateTimeOffset value) =>
            CreateOperation(_state.TimeIs(value));

        public MicroProcessorTestOperation CreateTimeHasPassedOperation(TimeSpan value)
        {
            if (value < TimeSpan.Zero)
            {
                throw NewNegativeTimeSpanException(value);
            }
            if (value == TimeSpan.Zero)
            {
                return new NullOperation();
            }
            return CreateOperation(_state.TimeHasPassed(value));
        }

        private MicroProcessorTestOperation CreateOperation(State newState) =>
            MoveToState(_state, newState).CreateGivenOperation();

        private State MoveToState(State expectedState, State newState)
        {
            if (Interlocked.CompareExchange(ref _state, newState, expectedState) == expectedState)
            {
                return newState;
            }
            throw NewUnexpectedStateException(expectedState, newState);
        }
        
        internal static Exception NewNegativeTimeUnitException(int value)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestTimeline_NegativeTimeUnit;
            var message = string.Format(messageFormat, value);
            return new ArgumentOutOfRangeException(nameof(value), message);
        }

        private static Exception NewNegativeTimeSpanException(TimeSpan value)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestTimeline_NegativeTimeSpan;
            var message = string.Format(messageFormat, value);
            return new ArgumentOutOfRangeException(nameof(value), message);
        }

        private static Exception NewInvalidTimeException(DateTimeOffset oldValue, DateTimeOffset value)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestTimeline_InvalidTime;
            var message = string.Format(messageFormat, value, oldValue);
            return new ArgumentOutOfRangeException(nameof(value), message);
        }

        private static Exception NewSpecificTimeNotAllowedException(DateTimeOffset value)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestTimeline_SpecificTimeNotAllowed;
            var message = string.Format(messageFormat, value);
            return new InvalidOperationException(message);
        }

        private static Exception NewUnexpectedStateException(State expectedState, State newState)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestTimeline_UnexpectedState;
            var message = string.Format(messageFormat, newState.GetType().FriendlyName(), expectedState.GetType().FriendlyName());
            return new InvalidOperationException(message);
        }
    }
}
