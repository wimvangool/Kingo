using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Succeeds_IfCorrectExceptionIsExpected : WriteScenarioTest<object>
    {
        private const string _Message = "TestMessage";

        protected override object When() =>
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
