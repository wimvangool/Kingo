using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server
{
    [TestClass]   
    public sealed class ScenarioTest
    {
        #region [====== Nested Types ======]

        private abstract class ScenarioUnderTest : Scenario<TheCommand>
        {                              
            public new bool ExceptionExpected
            {
                get { return base.ExceptionExpected; }
                set { base.ExceptionExpected = value; }
            }

            public Exception ExceptionThatWasThrown
            {
                get { return Exception; }
            }

            public new int DomainEventCount
            {
                get { return base.DomainEventCount; }
            }

            protected override IMessageProcessor MessageProcessor
            {
                get { return null; }
            }

            protected override IUnitTestFramework Framework
            {
                get { return MSTestFramework.Instance; }
            }

            public new object DomainEventAt(int index)
            {
                return base.DomainEventAt(index);
            }            
        }

        private sealed class ErroneousScenario : ScenarioUnderTest
        {
            private readonly Exception _exceptionToThrow;

            public ErroneousScenario(Exception exceptionToThrow)
            {
                _exceptionToThrow = exceptionToThrow;
            }

            protected override IMessageSequence Given()
            {
                throw _exceptionToThrow;
            }

            protected override TheCommand When()
            {
                return null;
            }
        }

        private sealed class ExceptionalFlowScenario : ScenarioUnderTest
        {
            private readonly Exception _exceptionToThrow;

            public ExceptionalFlowScenario(Exception exceptionToThrow)
            {
                _exceptionToThrow = exceptionToThrow;
            }

            protected override TheCommand When()
            {
                return new TheCommand()
                {
                    ExceptionToThrow = _exceptionToThrow
                };
            }
        }

        private sealed class HappyFlowScenario : ScenarioUnderTest
        {
            private readonly IEnumerable<DomainEvent> _messagesToPublish;

            public HappyFlowScenario(IEnumerable<DomainEvent> messagesToPublish)
            {
                _messagesToPublish = messagesToPublish;
            }

            protected override TheCommand When()
            {
                return new TheCommand()
                {
                    DomainEventsToPublish = _messagesToPublish
                };
            }
        }

        #endregion               

        #region [====== Execute ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void HandleWith_Throws_IfExceptionWasThrownByGiven()
        {
            var exception = new InvalidOperationException();
            var scenario = new ErroneousScenario(exception);

            try
            {
                scenario.ProcessWith(Processor);
            }
            finally
            {
                Assert.IsNull(scenario.ExceptionThatWasThrown);                
                Assert.AreEqual(0, scenario.DomainEventCount);
            } 
        }

        [TestMethod]        
        public void HandleWith_WillSetException_IfExceptionWasThrownByWhenAndWasExpected()
        {
            var exception = new InvalidOperationException();
            var scenario = new ExceptionalFlowScenario(exception);

            try
            {
                scenario.ExceptionExpected = true;
                scenario.ProcessWith(Processor);
            }
            finally
            {
                Assert.AreSame(exception, scenario.ExceptionThatWasThrown);
            }
        }

        [TestMethod]        
        public void HandleWith_WillHaveNoDomainEvents_IfExceptionWasThrownByWhenAndWasExpected()
        {
            var exception = new InvalidOperationException();
            var scenario = new ExceptionalFlowScenario(exception);

            try
            {
                scenario.ExceptionExpected = true;
                scenario.ProcessWith(Processor);
            }
            finally
            {               
                Assert.AreEqual(0, scenario.DomainEventCount);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void HandleWith_ThrowsAndWillSetException_IfExceptionWasThrownByWhenAndWasNotExpected()
        {
            var exception = new InvalidOperationException();
            var scenario = new ExceptionalFlowScenario(exception);

            try
            {
                scenario.ExceptionExpected = false;
                scenario.ProcessWith(Processor);
            }
            finally
            {
                Assert.AreSame(exception, scenario.ExceptionThatWasThrown);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void HandleWith_ThrowsAndWillHaveNoDomainEvents_IfExceptionWasThrownByWhenAndWasNotExpected()
        {
            var exception = new InvalidOperationException();
            var scenario = new ExceptionalFlowScenario(exception);

            try
            {
                scenario.ExceptionExpected = false;
                scenario.ProcessWith(Processor);
            }
            finally
            {                
                Assert.AreEqual(0, scenario.DomainEventCount);
            }
        }

        [TestMethod]
        public void HandleWith_WillSetExceptionToNull_IfNoExceptionWasThrownByWhen()
        {
            var messages = new[] { new DomainEvent(), new DomainEvent(),  };
            var scenario = new HappyFlowScenario(messages);

            scenario.ProcessWith(Processor);

            Assert.IsNull(scenario.ExceptionThatWasThrown);
        }

        [TestMethod]
        public void HandleWith_WillHaveDomainEvents_IfNoExceptionWasThrownByWhenAndSomeEventsWerePublished()
        {
            var messages = new[] { new DomainEvent(), new DomainEvent(),  };
            var scenario = new HappyFlowScenario(messages);

            scenario.ProcessWith(Processor);
                 
            Assert.AreEqual(messages.Length, scenario.DomainEventCount);
            Assert.AreSame(messages[0].GetType(), scenario.DomainEventAt(0).GetType());
            Assert.AreSame(messages[1].GetType(), scenario.DomainEventAt(1).GetType());
        }

        #endregion
        
        private static ScenarioTestProcessor Processor
        {
            get { return ScenarioTestProcessor.Instance; }
        }
    }
}
