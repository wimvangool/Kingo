using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Succeeds_IfCorrectExceptionAndInnerExceptionAreExpected : WriteScenarioTest<object>
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
            await Result.IsExceptionAsync<InternalServerErrorException>(exception =>
            {
                AssertInnerException<IllegalOperationException>(exception, innerException =>
                {
                    Assert.AreEqual(_Message, innerException.Message);
                });
            });
        }
    }
}
