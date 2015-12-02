using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]   
    public sealed class ScenarioTest
    {
        #region [====== Nested Types ======]

        private abstract class ScenarioUnderTest : Scenario<TheCommand>
        {                                                     
            protected override IMessageProcessor MessageProcessor
            {
                get { return null; }
            }

            protected override Exception NewScenarioFailedException(string message)
            {
                return new AssertFailedException(message);
            }

            public void VerifyThatEventsWerePublished(params object[] events)
            {
                VerifyThatDomainEventCount().IsEqualTo(events.Length);

                for (int index = 0; index < events.Length; index++)
                {
                    VerifyThatDomainEventAtIndex(index).IsEqualTo(events[index]);
                }                
            }        
    
            public void VerifyThatExceptionWasThrown<TException>(TException exception) where TException : FunctionalException
            {
                VerifyThatExceptionIsA<TException>().IsSameInstanceAs(exception);
            }
        }

        private sealed class ErroneousScenario : ScenarioUnderTest
        {
            private readonly Exception _exceptionToThrow;

            public ErroneousScenario(Exception exceptionToThrow) 
            {
                _exceptionToThrow = exceptionToThrow;
            }

            protected override IEnumerable<IMessageSequence> Given()
            {
                throw _exceptionToThrow;
            }

            protected override TheCommand When()
            {
                return null;
            }

            public override void Then() { }
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
                return new TheCommand
                {
                    ExceptionToThrow = _exceptionToThrow
                };
            }

            public override void Then() { }
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
                return new TheCommand
                {
                    DomainEventsToPublish = _messagesToPublish
                };
            }

            public override void Then() { }
        }

        #endregion 
        
        #region [====== Happy Flow ======]

        [TestMethod]
        public void TheNumberOfPublishedEventsIs_Succeeds_IfAllExpectedDomainEventsWerePublished()
        {            
            var messages = new[] { new DomainEvent(), new DomainEvent() };
            var scenario = new HappyFlowScenario(messages);

            scenario.ProcessWith(ScenarioTestProcessor.Instance);
            scenario.VerifyThatEventsWerePublished(messages[0], messages[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void TheNumberOfPublishedEventsIs_Fails_IfAnUnexpectedNumberOfDomainEventsWasPublished()
        {            
            var messages = new[] { new DomainEvent(), new DomainEvent() };
            var scenario = new HappyFlowScenario(messages);

            scenario.ProcessWith(ScenarioTestProcessor.Instance);
            scenario.VerifyThatEventsWerePublished(messages[0]);            
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void TheNumberOfPublishedEventsIs_Fails_IfTheWrongDomainEventsWerePublished()
        {            
            var messages = new[] { new DomainEvent(), new DomainEvent() };
            var scenario = new HappyFlowScenario(messages);

            scenario.ProcessWith(ScenarioTestProcessor.Instance);
            scenario.VerifyThatEventsWerePublished(messages[0], new DomainEvent());
        }

        #endregion      

        #region [====== Alternate Flow ======]

        [TestMethod]        
        public void ProcessWith_Throws_IfExceptionWasThrownByGiven()
        {            
            var exception = new InvalidOperationException();
            var scenario = new ErroneousScenario(exception);

            try
            {
                scenario.ProcessWith(ScenarioTestProcessor.Instance);

                Assert.Fail("Expected exception of type '{0}' was not thrown.", exception.GetType().Name);
            }
            catch (AggregateException aggregateException)
            {                
                Assert.AreEqual(1, aggregateException.InnerExceptions.Count);
                Assert.AreSame(exception, aggregateException.InnerExceptions[0]);
            }            
        }

        [TestMethod]        
        public void TheExceptionThatWasThrownIsOfType_Succeeds_IfExpectedExceptionWasThrownn()
        {            
            var exception = NewInvalidMessageException();
            var scenario = new ExceptionalFlowScenario(exception);

            scenario.ProcessWith(ScenarioTestProcessor.Instance);
            scenario.VerifyThatExceptionWasThrown(exception);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void TheExceptionThatWasThrownIsOfType_Fails_IfUnexpectedExceptionWasThrown()
        {            
            var exception = new InvalidOperationException();
            var scenario = new ExceptionalFlowScenario(exception);

            scenario.ProcessWith(ScenarioTestProcessor.Instance);            
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void TheExceptionThatWasThrownIsOfType_Fails_IfNoExceptionWasThrown()
        {            
            var scenario = new HappyFlowScenario(Enumerable.Empty<DomainEvent>());

            scenario.ProcessWith(ScenarioTestProcessor.Instance);
            scenario.VerifyThatExceptionWasThrown(NewInvalidMessageException());
        }

        private static InvalidMessageException NewInvalidMessageException()
        {
            return new InvalidMessageException(new TheCommand(), "Invalid command.");
        }

        #endregion                             
    }
}
