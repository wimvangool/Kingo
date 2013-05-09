using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YellowFlare.MessageProcessing
{
    [TestClass]   
    public sealed class ScenarioTest
    {
        #region [====== Nested Types ======]

        private abstract class ScenarioUnderTest : Scenario<TheCommand>
        {                   
            protected ScenarioUnderTest(IMessageProcessor processor) : base(processor) {}

            public new bool ExceptionExpected
            {
                get { return base.ExceptionExpected; }
                set { base.ExceptionExpected = value; }
            }

            public Exception ExceptionThatWasThrown
            {
                get { return Exception; }
            }

            public IEnumerable<object> DomainEventsThatWerePublished
            {
                get { return DomainEvents; }
            }

            protected override void Fail(string message)
            {
                Assert.Fail(message);
            }
        }

        private sealed class ErroneousScenario : ScenarioUnderTest
        {
            private readonly Exception _exceptionToThrow;

            public ErroneousScenario(IMessageProcessor processor, Exception exceptionToThrow) : base(processor)
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

            public ExceptionalFlowScenario(IMessageProcessor processor, Exception exceptionToThrow) : base(processor)
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
            private readonly IEnumerable<object> _messagesToPublish;

            public HappyFlowScenario(IMessageProcessor processor, IEnumerable<object> messagesToPublish) : base(processor)
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
        public void Execute_Throws_IfExceptionWasThrownByGiven()
        {
            var exception = new InvalidOperationException();
            var scenario = new ErroneousScenario(_Processor, exception);

            try
            {
                scenario.Execute();
            }
            finally
            {
                Assert.IsNull(scenario.ExceptionThatWasThrown);
                Assert.IsNotNull(scenario.DomainEventsThatWerePublished);
                Assert.AreEqual(0, scenario.DomainEventsThatWerePublished.Count());
            } 
        }

        [TestMethod]        
        public void Execute_WillSetException_IfExceptionWasThrownByWhenAndWasExpected()
        {
            var exception = new InvalidOperationException();
            var scenario = new ExceptionalFlowScenario(_Processor, exception);

            try
            {
                scenario.ExceptionExpected = true;
                scenario.Execute();
            }
            finally
            {
                Assert.AreSame(exception, scenario.ExceptionThatWasThrown);
            }
        }

        [TestMethod]        
        public void Execute_WillHaveNoDomainEvents_IfExceptionWasThrownByWhenAndWasExpected()
        {
            var exception = new InvalidOperationException();
            var scenario = new ExceptionalFlowScenario(_Processor, exception);

            try
            {
                scenario.ExceptionExpected = true;
                scenario.Execute();
            }
            finally
            {
                Assert.IsNotNull(scenario.DomainEventsThatWerePublished);
                Assert.AreEqual(0, scenario.DomainEventsThatWerePublished.Count());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Execute_ThrowsAndWillSetException_IfExceptionWasThrownByWhenAndWasNotExpected()
        {
            var exception = new InvalidOperationException();
            var scenario = new ExceptionalFlowScenario(_Processor, exception);

            try
            {
                scenario.ExceptionExpected = false;
                scenario.Execute();
            }
            finally
            {
                Assert.AreSame(exception, scenario.ExceptionThatWasThrown);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Execute_ThrowsAndWillHaveNoDomainEvents_IfExceptionWasThrownByWhenAndWasNotExpected()
        {
            var exception = new InvalidOperationException();
            var scenario = new ExceptionalFlowScenario(_Processor, exception);

            try
            {
                scenario.ExceptionExpected = false;
                scenario.Execute();
            }
            finally
            {
                Assert.IsNotNull(scenario.DomainEventsThatWerePublished);
                Assert.AreEqual(0, scenario.DomainEventsThatWerePublished.Count());
            }
        }

        [TestMethod]
        public void Execute_WillSetExceptionToNull_IfNoExceptionWasThrownByWhen()
        {
            var messages = new[] { new object(), new object() };
            var scenario = new HappyFlowScenario(_Processor, messages);

            scenario.Execute();

            Assert.IsNull(scenario.ExceptionThatWasThrown);
        }

        [TestMethod]
        public void Execute_WillHaveDomainEvents_IfNoExceptionWasThrownByWhenAndSomeEventsWerePublished()
        {
            var messages = new[] { new object(), new object() };
            var scenario = new HappyFlowScenario(_Processor, messages);

            scenario.Execute();

            Assert.IsNotNull(scenario.DomainEventsThatWerePublished);
            Assert.IsTrue(scenario.DomainEventsThatWerePublished.SequenceEqual(messages));
        }

        #endregion

        #region [====== Assembly Setup and Teardown ======]

        private static IMessageProcessor _Processor;        

        [ClassInitialize]
        public static void SetupClass(TestContext context)
        {
            _Processor = BuildProcessor();
        }  
     
        private static IMessageProcessor BuildProcessor()
        {
            var messageHandlerFactory = new MessageHandlerFactoryForUnity();
            messageHandlerFactory.RegisterMessageHandlers(Assembly.GetExecutingAssembly());
            return new MessageProcessor(messageHandlerFactory);
        }

        #endregion
    }
}
