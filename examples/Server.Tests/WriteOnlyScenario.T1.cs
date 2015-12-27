using System;
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

        protected override Exception NewScenarioFailedException(string errorMessage)
        {
            return new AssertFailedException(errorMessage);
        }
    }
}
