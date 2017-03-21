using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    [TestClass]
    public sealed class MessageValidationPipelineTest
    {
        #region [====== Messages ======]

        private abstract class MessageToValidate : IMessage
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
                return new ErrorInfo(Enumerable.Empty<KeyValuePair<string, string>>(), "Some error.");
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

        #endregion

        private MicroProcessorSpy _processor;

        [TestInitialize]
        public void Setup()
        {
            _processor = new MicroProcessorSpy();
            _processor.Add(new MessageValidationPipeline());
        }

        [TestMethod]
        public void Handle_DoesNothing_IfCommandIsValid()
        {
            AssertIsEmpty(_processor.Handle(new SomeCommand(true), (message, context) => { }));
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
                Assert.AreSame(someCommand, exception.FailedMessage);

                var invalidMessageException = exception.InnerException as InvalidMessageException;

                Assert.IsNotNull(invalidMessageException);                
                Assert.AreEqual("Message of type 'SomeCommand' is not valid: 1 validation error(s) found.", invalidMessageException.Message);
                throw;
            }
        }

        [TestMethod]
        public void Handle_DoesNothing_IfEventIsValid()
        {
            AssertIsEmpty(_processor.Handle(new SomeEvent(true), (message, context) => { }));
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
                Assert.AreSame(someEvent, exception.FailedMessage);

                var invalidMessageException = exception.InnerException as InvalidMessageException;

                Assert.IsNotNull(invalidMessageException);
                Assert.AreEqual("Message of type 'SomeEvent' is not valid: 1 validation error(s) found.", invalidMessageException.Message);
                throw;
            }
        }

        private static void AssertIsEmpty(IMessageStream stream)
        {
            Assert.IsNotNull(stream);
            Assert.AreEqual(0, stream.Count);
        }
    }
}
