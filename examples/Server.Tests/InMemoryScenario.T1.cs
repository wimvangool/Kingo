using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess
{
    [TestClass]
    public abstract class InMemoryScenario<TMessage> : Scenario<TMessage> where TMessage : class, IMessage
    {
        private readonly InMemoryProcessor _processor;

        protected InMemoryScenario()
        {
            _processor = new InMemoryProcessor();
        }

        protected override IMessageProcessor MessageProcessor
        {
            get { return _processor; }
        }                

        protected override Exception NewScenarioFailedException(string errorMessage, Exception innerException = null)
        {
            return new AssertFailedException(errorMessage, innerException);
        }

        protected static Session RandomSession()
        {
            return new Session(Guid.NewGuid(), "SomePlayer");
        }
    }
}
