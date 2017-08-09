using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class UnitTestBase_Fails_IfExceptionIsExpectedButExceptionForInnerExceptionIsNull : UnitTestBaseTest<object>
    {        
        protected override object WhenMessageIsHandled() =>
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
            try
            {
                await Result.IsExceptionOfTypeAsync<InternalServerErrorException>(exception =>
                {
                    AssertInnerExceptionIsOfType<IllegalOperationException>(null, innerException => { });
                });
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
