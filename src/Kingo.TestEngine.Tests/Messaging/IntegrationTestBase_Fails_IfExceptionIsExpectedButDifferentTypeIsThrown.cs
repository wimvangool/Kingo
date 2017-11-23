using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class IntegrationTestBase_Fails_IfExceptionIsExpectedButDifferentTypeIsThrown : IntegrationTestBaseTest<object>
    {
        protected override object ExecuteQuery(IMicroProcessorContext context) =>
             throw new InternalServerErrorException(new object());

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
                Assert.AreEqual("Expected exception of type 'BadRequestException' but exception of type 'InternalServerErrorException' was thrown instead. See inner exception for details.", exception.Message);
                throw;
            }
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}
