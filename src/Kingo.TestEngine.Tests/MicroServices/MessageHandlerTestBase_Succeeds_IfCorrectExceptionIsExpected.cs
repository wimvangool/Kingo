using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageHandlerTestBase_Succeeds_IfCorrectExceptionIsExpected : MessageHandlerTestBaseTest<object>
    {
        private const string _Message = "TestMessage";

        protected override object When() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler()
        {
            return MessageHandlerDecorator<object>.Decorate((message, context) =>
            {
                throw new BusinessRuleException(_Message);
            });
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            try
            {
                await ThenResult().IsExceptionOfTypeAsync<InternalServerErrorException>();
            }
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}
