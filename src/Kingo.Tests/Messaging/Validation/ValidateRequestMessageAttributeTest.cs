using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    [TestClass]
    public sealed class ValidateRequestMessageAttributeTest
    {
        #region [====== Messages ======]

        private static readonly ErrorInfo _Errors = new ErrorInfo(Enumerable.Empty<KeyValuePair<string, string>>(), "Some error.");

        private abstract class MessageToValidate : IRequestMessage
        {
            protected abstract bool IsValid
            {
                get;
            }

            public ErrorInfo Validate(bool haltOnFirstError = false)
            {
                if (IsValid)
                {
                    return ErrorInfo.Empty;
                }
                return _Errors;
            }
        }

        private sealed class SomeCommand : MessageToValidate
        {
            public SomeCommand(bool isValid)
            {
                IsValid = isValid;
            }

            protected override bool IsValid
            {
                get;
            }
        }

        private sealed class SomeEvent : MessageToValidate
        {
            public SomeEvent(bool isValid)
            {
                IsValid = isValid;
            }

            protected override bool IsValid
            {
                get;
            }
        }

        private sealed class ExternalEvent { }

        private sealed class ExternalEventValidator : IRequestMessageValidator<ExternalEvent>
        {
            public bool IsValid
            {
                get;
                set;
            }

            public ErrorInfo Validate(ExternalEvent message, bool haltOnFirstError = false)
            {
                if (IsValid)
                {
                    return ErrorInfo.Empty;
                }
                return _Errors;
            }
        }

        #endregion

        private ValidateRequestMessageAttribute _filter;
        private MicroProcessorSpy _processor;

        [TestInitialize]
        public void Setup()
        {
            _filter = new ValidateRequestMessageAttribute();
            _processor = new MicroProcessorSpy();
            _processor.Add(_filter);
        }

        #region [====== Handle ======]

        [TestMethod]
        public void Handle_HandlesCommand_IfCommandIsValid()
        {
            var someEvent = new object();

            AssertContains(_processor.Handle(new SomeCommand(true), (message, context) =>
            {
                context.EventBus.Publish(someEvent);
            }), someEvent);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void Handle_ThrowsBadRequestException_IfCommandIsNotValid()
        {
            var someCommand = new SomeCommand(false);

            try
            {
                _processor.Handle(someCommand, (message, context) => { });
            }
            catch (BadRequestException exception)
            {                
                var invalidMessageException = exception.InnerException as InvalidRequestException;

                Assert.IsNotNull(invalidMessageException);                
                Assert.AreEqual("Message of type 'SomeCommand' is not valid: Some error. 0 member error(s).", invalidMessageException.Message);
                throw;
            }
        }

        [TestMethod]
        public void Handle_HandlesEvent_IfEventIsValid()
        {
            var someEvent = new object();

            AssertContains(_processor.Handle(new SomeEvent(true), (message, context) =>
            {
                context.EventBus.Publish(someEvent);
            }), someEvent);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public void Handle_ThrowsInternalServerErrorsException_IfCommandIsNotValid()
        {
            var someEvent = new SomeEvent(false);

            try
            {
                _processor.Handle(someEvent, (message, context) => { });
            }
            catch (InternalServerErrorException exception)
            {                
                var invalidMessageException = exception.InnerException as InvalidRequestException;

                Assert.IsNotNull(invalidMessageException);
                Assert.AreEqual("Message of type 'SomeEvent' is not valid: Some error. 0 member error(s).", invalidMessageException.Message);
                throw;
            }
        }

        [TestMethod]
        public void Handle_HandlesMessage_IfMessageDoesNotImplementIMessageInterface_And_NoValidatorForTypeHasBeenRegistered()
        {
            var someEvent = new object();

            AssertContains(_processor.Handle(new object(), (message, context) =>
            {
                context.EventBus.Publish(someEvent);
            }), someEvent);
        }

        [TestMethod]
        public void Handle_HandlesMessage_IfMessageDoesNotImplementIMessageInterface_And_ValidatorReturnsNoErrors()
        {
            lock (_ExternalEventValidator)
            {
                _ExternalEventValidator.IsValid = true;

                var someEvent = new object();

                AssertContains(_processor.Handle(new ExternalEvent(), (message, context) =>
                {
                    context.EventBus.Publish(someEvent);
                }), someEvent);
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public void Handle_ThrowsInternalServerErrorsException_IfMessageDoesNotImplementIMessageInterface_And_ValidatorReturnsErrors()
        {
            lock (_ExternalEventValidator)
            {
                _ExternalEventValidator.IsValid = false;

                var someMessage = new ExternalEvent();

                try
                {
                    _processor.Handle(someMessage, (message, context) => { });
                }
                catch (InternalServerErrorException exception)
                {                    
                    var invalidMessageException = exception.InnerException as InvalidRequestException;

                    Assert.IsNotNull(invalidMessageException);
                    Assert.AreEqual("Message of type 'ExternalEvent' is not valid: Some error. 0 member error(s).", invalidMessageException.Message);
                    throw;
                }
            }
        }

        #endregion

        #region [====== Query<TMessageOut> ======]

        private sealed class SomeRequest : RequestMessageBase
        {
            public SomeRequest(int value)
            {
                Value = value;
            }

            public int Value
            {
                get;
            }

            protected override IRequestMessageValidator CreateMessageValidator()
            {
                var validator = new DelegateValidator<SomeRequest>((message, haltOnFirstError) =>
                {
                    var errorInfoBuilder = new ErrorInfoBuilder();

                    if (message.Value < 0)
                    {
                        errorInfoBuilder.Add("Value is invalid.", nameof(message.Value));
                    }
                    return errorInfoBuilder.BuildErrorInfo();
                });
                return base.CreateMessageValidator().Append(validator);
            }
        }

        [ValidateRequestMessage]
        private sealed class SomeQuery : IQuery<object>, IQuery<object, object>
        {
            public Task<object> ExecuteAsync(IMicroProcessorContext context) =>
                Task.FromResult(new object());

            public Task<object> ExecuteAsync(object message, IMicroProcessorContext context) =>
                Task.FromResult(message);
        }

        [TestMethod]
        public async Task ExecuteQuery_DoesNotThrow_IfThereIsNoRequestMessage()
        {
            await CreateProcessor().ExecuteAsync(new SomeQuery());
        }

        [TestMethod]
        public async Task ExecuteQuery_DoesNotThrow_IfThereIsRequestMessageIsValid()
        {
            await CreateProcessor().ExecuteAsync(new SomeRequest(0), new SomeQuery());
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task ExecuteQuery_Throws_IfThereIsRequestMessageIsValid()
        {
            await CreateProcessor().ExecuteAsync(new SomeRequest(-1), new SomeQuery());
        }

        private static MicroProcessor CreateProcessor() =>
            new MicroProcessor();

        #endregion

        private static readonly ExternalEventValidator _ExternalEventValidator = new ExternalEventValidator();

        static ValidateRequestMessageAttributeTest()
        {
            RequestMessageBase.Register(_ExternalEventValidator);
        }

        private static void AssertContains(IMessageStream stream, params object[] events)
        {
            Assert.IsNotNull(stream);
            Assert.AreEqual(events.Length, stream.Count);

            for (int index = 0; index < events.Length; index++)
            {
                Assert.AreSame(events[index], stream[index]);
            }
        }
    }
}
