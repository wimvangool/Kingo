using System;
using System.ComponentModel;
using System.ComponentModel.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SummerBreeze.ChessApplication
{
    [TestClass]
    public abstract class UnitTestScenario<TMessage> : Scenario<TMessage> where TMessage : class, IMessage<TMessage>
    {
        protected override IMessageProcessor MessageProcessor
        {
            get { return UnitTestProcessor.Instance; }
        }

        [TestInitialize]
        public virtual void Setup()
        {
            Execute();
        }

        protected override Exception NewScenarioFailedException(string message)
        {
            return new AssertFailedException(message);
        }
    }
}
