using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageHandlerTestBase_Fails_IfExceptionIsExpectedButExceptionForInnerExceptionIsNull : MessageHandlerTestBaseTest<object>
    {        
        protected override object When() =>
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
                await ThenResult().IsExceptionOfTypeAsync<InternalServerErrorException>(exception =>
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
