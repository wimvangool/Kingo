using System;
using Kingo.BuildingBlocks.ComponentModel;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication
{
    [TestClass]
    public abstract class UnitTestScenario<TMessage> : Scenario<TMessage>
        where TMessage : class, IMessage<TMessage>
    {
        private readonly UnitTestProcessor _processor;

        protected UnitTestScenario()
        {
            _processor = new UnitTestProcessor();
        }

        protected override IMessageProcessor MessageProcessor
        {
            get { return _processor; }
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