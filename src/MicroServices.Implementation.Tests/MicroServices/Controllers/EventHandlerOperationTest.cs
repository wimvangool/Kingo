using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class EventHandlerOperationTest : MessageHandlerOperationTest
    {
        #region [====== Exception Handling ======]

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException), AllowDerivedTypes = true)]
        public override async Task HandleMessageAsync_ThrowsExpectedException_IfOperationThrowsMessageHandlerOperationException()
        {
            var exceptionToThrow = new BusinessRuleException();

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public override async Task HandleMessageAsync_ThrowsExpectedException_IfOperationThrowsBadRequestException()
        {
            var exceptionToThrow = new BadRequestException(null, null);

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        #endregion

        #region [====== HandleMessageAsync ======]

        protected override MessageKind MessageKind =>
            MessageKind.Event;

        protected override Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMicroProcessor processor, Action<TMessage, IMessageHandlerOperationContext> messageHandler, TMessage message, CancellationToken? token = null) =>
            processor.HandleEventAsync(messageHandler, message, MessageHeader.Unspecified, token);

        protected override Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMicroProcessor processor, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, TMessage message, CancellationToken? token = null) =>
            processor.HandleEventAsync(messageHandler, message, MessageHeader.Unspecified, token);

        protected override Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMicroProcessor processor, IMessageHandler<TMessage> messageHandler, TMessage message, CancellationToken? token = null) =>
            processor.HandleEventAsync(messageHandler, message, MessageHeader.Unspecified, token);

        #endregion
    }
}
