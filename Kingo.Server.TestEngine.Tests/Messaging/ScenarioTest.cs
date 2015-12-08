using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                get { return ScenarioTestProcessor.Instance; }
            }

            protected override Exception NewScenarioFailedException(string message)
            {
                return new AssertFailedException(message);
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
        }

        private sealed class AlternateFlowScenario : ScenarioUnderTest
        {
            private readonly Exception _exceptionToThrow;

            public AlternateFlowScenario(Exception exceptionToThrow)
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

            public override async Task ExecuteAsync()
            {
                await SetupAlternateFlow().Expect<InvalidMessageException>().ExecuteAsync();
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

            protected override TheCommand When()
            {
                return new TheCommand
                {
                    DomainEventsToPublish = _messagesToPublish
                };
            }

            public override async Task ExecuteAsync()
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
                await SetupHappyFlow(3).ExecuteAsync();
            }

            private async Task ExecuteInvalidExpectedEventTypeCaseAsync()
            {
                await SetupHappyFlow(2)
                    .Expect<TheCommand>(0)
                    .Expect<DomainEvent>(1)
                    .ExecuteAsync();
            }

            private async Task ExecuteInvalidExpectedEventValuesCaseAsync()
            {
                await SetupHappyFlow(2)
                    .Expect<DomainEvent>(0, validator => validator.VerifyThat(m => m.Value).IsEqualTo(_messagesToPublish[0].Value))
                    .Expect<DomainEvent>(1, validator => validator.VerifyThat(m => m.Value).IsNotEqualTo(_messagesToPublish[1].Value))
                    .ExecuteAsync();
            }

            private async Task ExecuteDefaultCaseAsync()
            {
                await SetupHappyFlow(2)
                    .Expect<DomainEvent>(0, validator => validator.VerifyThat(m => m.Value).IsEqualTo(_messagesToPublish[0].Value))
                    .Expect<DomainEvent>(1, validator => validator.VerifyThat(m => m.Value).IsEqualTo(_messagesToPublish[1].Value))
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

            await scenario.ExecuteAsync();
        }
       
        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task Execute_Throws_IfAnUnexpectedNumberOfDomainEventsWasPublished()
        {            
            var messages = new[] { new DomainEvent(), new DomainEvent() };
            var scenario = new HappyFlowScenario(messages, HappyFlowScenarioCase.InvalidEventCount);

            await scenario.ExecuteAsync();          
        }
   
        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task Execute_Throws_IfTheWrongDomainEventsWerePublished()
        {            
            var messages = new[] { new DomainEvent(), new DomainEvent() };
            var scenario = new HappyFlowScenario(messages, HappyFlowScenarioCase.InvalidExpectedEventType);

            await scenario.ExecuteAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task Execute_Throws_IfValidationOfPublishedEventFails()
        {
            var messages = new[] { new DomainEvent(), new DomainEvent() };
            var scenario = new HappyFlowScenario(messages, HappyFlowScenarioCase.InvalidExpectedEventValues);

            await scenario.ExecuteAsync();
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
                await scenario.ExecuteAsync();

                Assert.Fail("Expected exception of type '{0}' was not thrown.", exceptionToThrow.GetType().Name);
            }
            catch (InvalidOperationException exception)
            {                               
                Assert.AreSame(exceptionToThrow, exception);
            }            
        }

        [TestMethod]        
        public async Task Execute_Succeeds_IfAlternateFlowIsSetup_And_ExpectedExceptionWasThrownn()
        {            
            var exception = NewInvalidMessageException();
            var scenario = new AlternateFlowScenario(exception);

            await scenario.ExecuteAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Execute_Throws_IfUnexpectedTechnicalExceptionWasThrown()
        {            
            var exception = new InvalidOperationException();
            var scenario = new AlternateFlowScenario(exception);

            await scenario.ExecuteAsync();           
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task Execute_Throws_IfUnexpectedFunctionalExceptionWasThrown()
        {
            var exception = new CommandExecutionException(new object());
            var scenario = new AlternateFlowScenario(exception);

            await scenario.ExecuteAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public async Task Execute_Throws_IfAlternateFlowIsSetup_And_NoExceptionWasThrown()
        {            
            var scenario = new AlternateFlowScenario(null);

            await scenario.ExecuteAsync();
        }

        private static InvalidMessageException NewInvalidMessageException()
        {
            return new InvalidMessageException(new TheCommand(), "Invalid command.");
        }

        #endregion                             
    }
}
