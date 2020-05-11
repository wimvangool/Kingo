using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class CommandHandlerOperationTest : MessageHandlerOperationTest
    {
        #region [====== Exception Handling ======]

        [TestMethod]
        [ExpectedException(typeof(BadRequestException), AllowDerivedTypes = true)]
        public override async Task HandleMessageAsync_ThrowsExpectedException_IfOperationThrowsMessageHandlerOperationException()
        {
            var exceptionToThrow = new BusinessRuleViolationException();

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (BadRequestException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public override async Task HandleMessageAsync_ThrowsExpectedException_IfOperationThrowsBadRequestException()
        {
            var exceptionToThrow = new BadRequestException(MicroProcessorOperationStackTrace.Empty);

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (BadRequestException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
        }

        #endregion

        #region [====== HandleMessageAsync ======]

        protected override MessageKind MessageKind =>
            MessageKind.Command;

        protected override Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMicroProcessor processor, Action<TMessage, IMessageHandlerOperationContext> messageHandler, TMessage message, CancellationToken? token = null) =>
            processor.ExecuteCommandAsync(messageHandler, message, MessageHeader.Unspecified, token);

        protected override Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMicroProcessor processor, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, TMessage message, CancellationToken? token = null) =>
            processor.ExecuteCommandAsync(messageHandler, message, MessageHeader.Unspecified, token);

        protected override Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMicroProcessor processor, IMessageHandler<TMessage> messageHandler, TMessage message, CancellationToken? token = null) =>
            processor.ExecuteCommandAsync(messageHandler, message, MessageHeader.Unspecified, token);

        #endregion
    }
}
