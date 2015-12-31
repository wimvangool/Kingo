using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess
{
    [TestClass]
    public abstract class MemoryScenario<TMessage> : Scenario<TMessage> where TMessage : class, IMessage<TMessage>
    {
        private readonly MemoryProcessor _processor;

        protected MemoryScenario()
        {
            _processor = new MemoryProcessor();
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
