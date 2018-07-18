using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class UnitTestBase_Succeeds_IfCorrectExceptionAndInnerExceptionAreExpected : UnitTestBaseTest<object>
    {
        private const string _Message = "TestMessage";

        protected override object WhenMessageIsHandled() =>
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
                await Result.IsExceptionOfTypeAsync<InternalServerErrorException>(exception =>
                {
                    AssertInnerExceptionIsOfType<BusinessRuleException>(exception, innerException =>
                    {
                        Assert.AreEqual(_Message, innerException.Message);
                    });
                });
            }
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}
