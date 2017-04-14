using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Fails_IfExceptionIsExpectedButAssertCallbackForInnerExceptionIsNull : WriteScenarioTest<object>
    {        
        protected override object When() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler()
        {
            return MessageHandler<object>.FromDelegate((message, context) =>
            {
                throw new IllegalOperationException("Test");
            });
        }

        [TestMethod]        
        [ExpectedException(typeof(ArgumentNullException))]
        public override async Task ThenAsync()
        {
            await Result.IsExceptionAsync<InternalServerErrorException>(exception =>
            {
                AssertInnerException<IllegalOperationException>(exception, null);
            });
        }        
    }
}
