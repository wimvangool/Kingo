using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class ReadScenario_Succeeds_IfCorrectExceptionAndInnerExceptionAreExpected : ReadScenarioTest<object>
    {
        private const string _Message = "TestMessage";

        protected override object ExecuteQuery(IMicroProcessorContext context)
        {
            throw new BadRequestException(new object(), _Message, new Exception(_Message));
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            try
            {
                await Result.IsExceptionOfTypeAsync<BadRequestException>(exception =>
                {
                    AssertInnerExceptionIsOfType<Exception>(exception, innerException =>
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
