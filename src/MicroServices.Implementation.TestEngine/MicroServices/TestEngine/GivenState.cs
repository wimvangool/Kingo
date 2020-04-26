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

            protected GivenState GivenState =>
                _givenState;

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

        #region [====== MessageState ======]

        private abstract class MessageState : GivenTimeOrMessageState
        {
            protected MessageState(GivenState givenState) : base(givenState)
            {
                // Whenever a new message-operation is scheduled, the timeline must commit to
                // absolute or relative time, because this can no longer change after a message-operation
                // has been executed.
                // For example, we don't want to run the first operation in relative time and then allow
                // a specific time (absolute time) to be set before the second operation runs, since that
                // can result in weird behavior (i.e. going backwards in time).
                givenState._timeline.CommitToAbsoluteOrRelativeTime();
            }
        }

        #endregion

        #region [====== CommandState<TCommand> ======]

        private sealed class CommandState<TCommand> : MessageState, IGivenCommandState<TCommand>
        {
            public CommandState(GivenState givenState) : base(givenState) { }

            public void IsExecutedBy<TMessageHandler>(TCommand message) where TMessageHandler : class, IMessageHandler<TCommand> =>
                IsExecutedBy<TMessageHandler>(ConfigureMessage(message));

            public void IsExecutedBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TCommand> =>
                AddOperation(new CommandOperation<TCommand, TMessageHandler>(configurator));

            public void IsExecutedBy(IMessageHandler<TCommand> messageHandler, Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator) =>
                AddOperation(new CommandOperation<TCommand>(messageHandler, configurator));
        }

        #endregion

        #region [====== EventState<TEvent> ======]

        private sealed class EventState<TEvent> : MessageState, IGivenEventState<TEvent>
        {
            public EventState(GivenState givenState) : base(givenState) { }

            public void IsHandledBy<TEventHandler>(TEvent message) where TEventHandler : class, IMessageHandler<TEvent> =>
                IsHandledBy<TEventHandler>(ConfigureMessage(message));

            public void IsHandledBy<TEventHandler>(Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator) where TEventHandler : class, IMessageHandler<TEvent> =>
                AddOperation(new EventOperation<TEvent, TEventHandler>(configurator));

            public void IsHandledBy(IMessageHandler<TEvent> messageHandler, Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator) =>
                AddOperation(new EventOperation<TEvent>(messageHandler, configurator));
        }

        #endregion

        #region [====== RequestState<TRequest> ======]

        private sealed class RequestState : MessageState, IGivenRequestState
        {
            public RequestState(GivenState givenState) : base(givenState) { }

            public IGivenResponseState<TResponse> Returning<TResponse>() =>
                Test.MoveToState(this, new ResponseState<TResponse>(GivenState));
        }

        #endregion

        #region [====== RequestState<TRequest> ======]

        private sealed class RequestState<TRequest> : MessageState, IGivenRequestState<TRequest>
        {
            public RequestState(GivenState givenState) : base(givenState) { }

            public IGivenResponseState<TRequest, TResponse> Returning<TResponse>() =>
                Test.MoveToState(this, new ResponseState<TRequest, TResponse>(GivenState));
        }

        #endregion

        #region [====== ResponseState<TResponse> ======]

        private sealed class ResponseState<TResponse> : MessageState, IGivenResponseState<TResponse>
        {
            public ResponseState(GivenState givenState) : base(givenState) { }

            public void IsExecutedBy<TQuery>(Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator = null) where TQuery : class, IQuery<TResponse> =>
                AddOperation(new QueryTestOperation1<TResponse, TQuery>(configurator));

            public void IsExecutedBy(IQuery<TResponse> query, Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator = null) =>
                AddOperation(new QueryTestOperation1<TResponse>(configurator, query));
        }

        #endregion

        #region [====== ResponseState<TRequest, TResponse> ======]

        private sealed class ResponseState<TRequest, TResponse> : MessageState, IGivenResponseState<TRequest, TResponse>
        {
            public ResponseState(GivenState givenState) : base(givenState) { }

            public void IsExecutedBy<TQuery>(TRequest request) where TQuery : class, IQuery<TRequest, TResponse> =>
                IsExecutedBy<TQuery>(ConfigureRequest(request));

            public void IsExecutedBy<TQuery>(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) where TQuery : class, IQuery<TRequest, TResponse> =>
                AddOperation(new QueryTestOperation2<TRequest, TResponse, TQuery>(configurator));

            public void IsExecutedBy(IQuery<TRequest, TResponse> query, Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) =>
                AddOperation(new QueryTestOperation2<TRequest, TResponse>(configurator, query));
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
            $"Scheduling new operation... ({_givenOperations}) | {_timeline}";

        #region [====== Time ======]

        public void TimeIs(DateTimeOffset value) =>
            AddGivenOperation(this, _timeline.CreateTimeIsOperation(value));

        public IGivenTimeHasPassedForState TimeHasPassedFor(int value) =>
            _test.MoveToState(this, new TimeHasPassedForState(this, value));

        public void TimeHasPassed(TimeSpan offset) =>
            AddGivenOperation(this, _timeline.CreateTimeHasPassedOperation(offset));

        private void AddGivenOperation(MicroProcessorTestState expectedState, MicroProcessorTestOperation givenOperation) =>
            _test.MoveToState(expectedState, new ReadyToConfigureTestState(_test, _timeline, _givenOperations.Enqueue(givenOperation)));

        #endregion

        #region [====== Commands & Events ======]

        public IGivenCommandState<TCommand> Command<TCommand>() =>
            _test.MoveToState(this, new CommandState<TCommand>(this));

        public IGivenEventState<TEvent> Event<TEvent>() =>
            _test.MoveToState(this, new EventState<TEvent>(this));

        #endregion

        #region [====== Requests ======]

        public IGivenRequestState Request() =>
            _test.MoveToState(this, new RequestState(this));

        public IGivenRequestState<TRequest> Request<TRequest>() =>
            _test.MoveToState(this, new RequestState<TRequest>(this));

        #endregion
    }
}
