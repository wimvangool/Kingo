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
            return MessageHandlerDecorator<object>.Decorate((message, context) =>
            {
                throw new BusinessRuleException("Test");
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
                    AssertInnerExceptionIsOfType<BusinessRuleException>(null, innerException => { });
                });
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
