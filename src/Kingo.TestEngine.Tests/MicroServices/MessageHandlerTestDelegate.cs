using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerTestDelegate : MessageHandlerTest<object>
    {
        private readonly GivenStatementCollection _givenStatements;

        public MessageHandlerTestDelegate()
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = async (messageProcessor, testContext) =>
            {
                await messageProcessor.HandleAsync(new object(), (message, context) => { });
            };
            _thenStatement = (message, result, context) => { };
        }

        #region [====== Given ======]

        public MessageHandlerTestDelegate Given(Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IMessageHandlerTestProcessor processor, MicroProcessorTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IMessageProcessor<object>, MicroProcessorTestContext, Task> _whenStatement;

        public MessageHandlerTestDelegate When(Func<IMessageProcessor<object>, MicroProcessorTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IMessageProcessor<object> processor, MicroProcessorTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion        

        #region [====== Then ======]

        private Action<object, IMessageHandlerResult, MicroProcessorTestContext> _thenStatement;

        public MessageHandlerTestDelegate ThenResultIsEventStream() =>
            Then((message, result, context) => result.IsEventStream());

        public MessageHandlerTestDelegate Then(Action<object, IMessageHandlerResult, MicroProcessorTestContext> thenStatement)
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(object message, IMessageHandlerResult result, MicroProcessorTestContext context) =>
            _thenStatement.Invoke(message, result, context);

        #endregion
    }
}
