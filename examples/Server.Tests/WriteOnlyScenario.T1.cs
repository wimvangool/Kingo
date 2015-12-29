using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess
{
    [TestClass]
    public abstract class WriteOnlyScenario<TMessage> : Scenario<TMessage> where TMessage : class, IMessage<TMessage>
    {
        private readonly WriteOnlyProcessor _processor;

        protected WriteOnlyScenario()
        {
            _processor = new WriteOnlyProcessor();
        }

        protected override IMessageProcessor MessageProcessor
        {
            get { return _processor; }
        }

        protected override async Task HandleAsync(IMessageProcessor processor, TMessage message)
        {
            var session = CreateSession();
            if (session == null)
            {
                await base.HandleAsync(processor, message);
            }
            else
            {
                using (Session.CreateSessionScope(session))
                {
                    await base.HandleAsync(processor, message);
                }
            }
        }

        protected virtual Session CreateSession()
        {
            return null;
        }

        protected override Exception NewScenarioFailedException(string errorMessage)
        {
            return new AssertFailedException(errorMessage);
        }
    }
}
