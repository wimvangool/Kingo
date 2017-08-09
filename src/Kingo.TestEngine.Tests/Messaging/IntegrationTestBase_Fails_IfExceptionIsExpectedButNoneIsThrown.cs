using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class IntegrationTestBase_Fails_IfExceptionIsExpectedButNoneIsThrown : IntegrationTestBaseTest<object>
    {
        protected override object ExecuteQuery(IMicroProcessorContext context) =>
            new object();

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            try
            {
                await Result.IsExceptionOfTypeAsync<BadRequestException>();
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("Expected exception of type 'BadRequestException', but no exception was thrown.", exception.Message);
                throw;
            }
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}
