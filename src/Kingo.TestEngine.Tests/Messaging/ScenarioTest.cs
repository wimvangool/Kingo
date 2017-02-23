using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Messaging.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]   
    public sealed class ScenarioTest
    {
        #region [====== Nested Types ======]

        private abstract class ScenarioUnderTest : Scenario<TheCommand>
        {
            protected ScenarioUnderTest()
            {
                TestEngine = new MSTestEngine();
                MessageProcessor = new ScenarioTestProcessor();
            }

            protected override ITestEngine TestEngine
            {
                get;
            }

            protected override IMessageProcessor MessageProcessor
            {
                get;
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

            protected override MessageToHandle<TheCommand> When()
            {
                return null;
            }           
        }

        private sealed class AlternateFlowScenario : ScenarioUnderTest
        {
            private readonly Exception _exceptionToThrow;

            public AlternateFlowScenario(Exception exceptionToThrow)
            {
                _exceptionToThrow = exceptionToThrow;
            }

            protected override MessageToHandle<TheCommand> When()
            {
                return new TheCommand
                {
                    ExceptionToThrow = _exceptionToThrow
                };
            }

            public override async Task ThenAsync()
            {
                await Exception().Expect<InvalidMessageException>().ExecuteAsync();
            }
        }

        private sealed class HappyFlowScenario : ScenarioUnderTest
        {
            private readonly IReadOnlyList<DomainEvent> _messagesToPublish;
            private readonly HappyFlowScenarioCase _testcase;

            public HappyFlowScenario(IEnumerable<DomainEvent> messagesToPublish, HappyFlowScenarioCase testcase)
            {
                _messagesToPublish = messagesToPublish.ToArray();
                _testcase = testcase;
            }

            protected override MessageToHandle<TheCommand> When()
            {
                return new TheCommand
                {
                    DomainEventsToPublish = _messagesToPublish
                };
            }

            public override async Task ThenAsync()
            {
                switch (_testcase)
                {
                    case HappyFlowScenarioCase.InvalidEventCount:
                        await ExecuteInvalidEventCountCaseAsync();
                        return;
                    case HappyFlowScenarioCase.InvalidExpectedEventType:
                        await ExecuteInvalidExpectedEventTypeCaseAsync();
                        return;
                    case HappyFlowScenarioCase.InvalidExpectedEventValues:
                        await ExecuteInvalidExpectedEventValuesCaseAsync();
                        return;
                    default:
                        await ExecuteDefaultCaseAsync();
                        return;
                }                
            }

            private async Task ExecuteInvalidEventCountCaseAsync()
            {
                await Events().Expect<DomainEvent>().ExecuteAsync();
            }

            private async Task ExecuteInvalidExpectedEventTypeCaseAsync()
            {
                await Events()
                    .Expect<TheCommand>()
                    .Expect<DomainEvent>()
                    .ExecuteAsync();
            }

            private async Task ExecuteInvalidExpectedEventValuesCaseAsync()
            {
                await Events()
                    .Expect<DomainEvent>(validator => validator.VerifyThat(m => m.Value).IsEqualTo(_messagesToPublish[0].Value))
                    .Expect<DomainEvent>(validator => validator.VerifyThat(m => m.Value).IsNotEqualTo(_messagesToPublish[1].Value))
                    .ExecuteAsync();
            }

            private async Task ExecuteDefaultCaseAsync()
            {
                await Events()
                    .Expect<DomainEvent>(validator => validator.VerifyThat(m => m.Value).IsEqualTo(_messagesToPublish[0].Value))
                    .Expect<DomainEvent>(validator => validator.VerifyThat(m => m.Value).IsEqualTo(_messagesToPublish[1].Value))
                    .ExecuteAsync();
            }
        }

        private enum HappyFlowScenarioCase
        {            
            Default,

            InvalidEventCount,

            InvalidExpectedEventType,

            InvalidExpectedEventValues
        }

        #endregion 
        
        #region [====== Happy Flow ======]

        [TestMethod]
        public async Task Execute_Succeeds_IfHappyFlowIsSetup_And_AllExpectedDomainEventsWerePublished()
        {            
            var messages = new[] { new DomainEvent(), new DomainEvent() };
            var scenario = new HappyFlowScenario(messages, HappyFlowScenarioCase.Default);

            await scenario.ThenAsync();
        }
       
        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task Execute_Throws_IfAnUnexpectedNumberOfDomainEventsWasPublished()
        {            
            var messages = new[] { new DomainEvent(), new DomainEvent() };
            var scenario = new HappyFlowScenario(messages, HappyFlowScenarioCase.InvalidEventCount);

            await scenario.ThenAsync();          
        }
   
        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task Execute_Throws_IfTheWrongDomainEventsWerePublished()
        {            
            var messages = new[] { new DomainEvent(), new DomainEvent() };
            var scenario = new HappyFlowScenario(messages, HappyFlowScenarioCase.InvalidExpectedEventType);

            await scenario.ThenAsync();
        }

        [TestMethod]        
        public async Task Execute_Throws_IfValidationOfPublishedEventFails()
        {
            var messages = new[] { new DomainEvent(1), new DomainEvent(2) };
            var scenario = new HappyFlowScenario(messages, HappyFlowScenarioCase.InvalidExpectedEventValues);

            try
            {
                await scenario.ThenAsync();
            }
            catch (AssertFailedException exception)
            {
                Assert.AreEqual("PublishedEvents[1].Value (2) must not be equal to '2'.", exception.Message);               
            }            
        }

        #endregion      

        #region [====== Alternate Flow ======]

        [TestMethod]        
        public async Task Execute_Throws_IfExceptionWasThrownByGiven()
        {            
            var exceptionToThrow = new InvalidOperationException();
            var scenario = new ErroneousScenario(exceptionToThrow);

            try
            {
                await scenario.ThenAsync();

                Assert.Fail("Expected exception of type '{0}' was not thrown.", exceptionToThrow.GetType().Name);
            }
            catch (AssertFailedException exception)
            {                               
                Assert.AreSame(exceptionToThrow, exception.InnerException);
            }            
        }

        [TestMethod]        
        public async Task Execute_Succeeds_IfAlternateFlowIsSetup_And_ExpectedExceptionWasThrownn()
        {            
            var exception = NewInvalidMessageException();
            var scenario = new AlternateFlowScenario(exception);

            await scenario.ThenAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Execute_Throws_IfUnexpectedTechnicalExceptionWasThrown()
        {            
            var exception = new InvalidOperationException();
            var scenario = new AlternateFlowScenario(exception);

            await scenario.ThenAsync();           
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task Execute_Throws_IfUnexpectedFunctionalExceptionWasThrown()
        {
            var exception = new CommandExecutionException(new object());
            var scenario = new AlternateFlowScenario(exception);

            await scenario.ThenAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task Execute_Throws_IfAlternateFlowIsSetup_And_NoExceptionWasThrown()
        {            
            var scenario = new AlternateFlowScenario(null);

            await scenario.ThenAsync();
        }

        private static InvalidMessageException NewInvalidMessageException()
        {
            return new InvalidMessageException(new TheCommand(), "Invalid command.");
        }

        #endregion                             
    }
}
