using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server
{
    [TestClass]   
    public sealed class ScenarioTest
    {
        #region [====== Nested Types ======]

        private abstract class ScenarioUnderTest : Scenario<TheCommand>
        {
            private readonly bool _exceptionExpected;
            private readonly int _expectedDomainEventCount;
        
            protected ScenarioUnderTest(bool exceptionExpected, int expectedEventCount)
            {
                _exceptionExpected = exceptionExpected;
                _expectedDomainEventCount = expectedEventCount;
            }

            protected override bool ExceptionExpected
            {
                get { return _exceptionExpected; }
            }

            protected override int ExpectedDomainEventCount
            {
                get { return _expectedDomainEventCount; }
            }

            public Exception ExceptionThatWasThrown
            {
                get { return Exception; }
            }            

            protected override IMessageProcessor MessageProcessor
            {
                get { return null; }
            }            

            protected override void Fail(string message)
            {
                Assert.Fail(message);
            }

            public void VerifyThatEventsWerePublished(params object[] events)
            {
                for (int index = 0; index < events.Length; index++)
                { 
                    VerifyThatEventAtIndex(index).IsEqualTo(events[index]);
                }
            }            
        }

        private sealed class ErroneousScenario : ScenarioUnderTest
        {
            private readonly Exception _exceptionToThrow;

            public ErroneousScenario(bool exceptionExpected, Exception exceptionToThrow) : base(exceptionExpected, 0)
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

            public ExceptionalFlowScenario(bool exceptionExpected, Exception exceptionToThrow) : base(exceptionExpected, 0)
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

            public HappyFlowScenario(bool exceptionExpected, IEnumerable<DomainEvent> messagesToPublish) : base(exceptionExpected, messagesToPublish.Count())
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
            var scenario = new ErroneousScenario(false, exception);

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
            var scenario = new ExceptionalFlowScenario(true, exception);

            try
            {                
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
            var scenario = new ExceptionalFlowScenario(true, exception);

            try
            {                
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
            var scenario = new ExceptionalFlowScenario(false, exception);

            try
            {                
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
            var scenario = new ExceptionalFlowScenario(false, exception);

            try
            {                
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
            var scenario = new HappyFlowScenario(false, messages);

            scenario.ProcessWith(Processor);

            Assert.IsNull(scenario.ExceptionThatWasThrown);
        }        

        [TestMethod]
        public void VerifyThat_WillSuccesfullyVerifyResult_IfAssumptionsAreCorrect()
        {
            var messages = new[] { new DomainEvent(), new DomainEvent(), };
            var scenario = new HappyFlowScenario(false, messages);

            scenario.ProcessWith(Processor);
            scenario.VerifyThatEventsWerePublished(messages[0], messages[1]);            
        }

        [TestMethod]        
        public void VerifyThat_WillFailAsExpected_IfAssumptionsAreNotCorrect()
        {
            var messages = new[] { new DomainEvent(), new DomainEvent(), };
            var scenario = new HappyFlowScenario(false, messages);

            scenario.ProcessWith(Processor);

            try
            {
                scenario.VerifyThatEventsWerePublished(new object());

                Assert.Fail("The expected exception was not thrown.");
            }
            catch (AssertFailedException exception)
            {
                Assert.IsTrue(exception.Message.Contains("DomainEvents[0]"));
            }            
        }

        #endregion
        
        private static ScenarioTestProcessor Processor
        {
            get { return ScenarioTestProcessor.Instance; }
        }
    }
}
