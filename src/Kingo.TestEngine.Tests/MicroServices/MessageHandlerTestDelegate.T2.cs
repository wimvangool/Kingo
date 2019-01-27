using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerTestDelegate<TMessage, TEventStream> : MicroProcessorTestDelegate, IMessageHandlerTest<TMessage, TEventStream>
        where TEventStream : EventStream
    {                        
        public MessageHandlerTestDelegate()
        {
            _whenStatement = (processor, context) => Task.CompletedTask;
            _thenStatement = (message, result, context) => { };
        }

        #region [====== Given ======]

        public new MessageHandlerTestDelegate<TMessage, TEventStream> Given(Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task> givenStatement)
        {
            base.Given(givenStatement);
            return this;
        }

        #endregion

        #region [====== When ======]

        private Func<IMessageProcessor<TMessage>, MicroProcessorTestContext, Task> _whenStatement;

        public MessageHandlerTestDelegate<TMessage, TEventStream> When(Func<IMessageProcessor<TMessage>, MicroProcessorTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        Task IMessageHandlerTest<TMessage, TEventStream>.WhenAsync(IMessageProcessor<TMessage> processor, MicroProcessorTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion

        #region [====== Then ======]

        private Action<TMessage, IMessageHandlerResult<TEventStream>, MicroProcessorTestContext> _thenStatement;

        public MessageHandlerTestDelegate<TMessage, TEventStream> Then(Action<TMessage, IMessageHandlerResult<TEventStream>, MicroProcessorTestContext> thenStatement)
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        void IMessageHandlerTest<TMessage, TEventStream>.Then(TMessage message, IMessageHandlerResult<TEventStream> result, MicroProcessorTestContext context) =>
            _thenStatement.Invoke(message, result, context);

        #endregion
    }
}
