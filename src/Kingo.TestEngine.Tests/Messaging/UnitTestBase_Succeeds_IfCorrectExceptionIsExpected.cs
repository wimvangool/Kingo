using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class UnitTestBase_Succeeds_IfCorrectExceptionIsExpected : UnitTestBaseTest<object>
    {
        private const string _Message = "TestMessage";

        protected override object WhenMessageIsHandled() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler()
        {
            return MessageHandler<object>.FromDelegate((message, context) =>
            {
                throw new IllegalOperationException(_Message);
            });
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            try
            {
                await Result.IsExceptionOfTypeAsync<InternalServerErrorException>();
            }
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}
