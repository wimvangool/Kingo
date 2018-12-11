using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class QueryTestBase_Fails_IfExceptionIsExpectedButDifferentTypeIsThrown : QueryTestBaseTest<object>
    {
        protected override object ExecuteQuery(IMicroProcessorContext context) =>
             throw new InternalServerErrorException();

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            try
            {
                await ThenResult().IsExceptionOfTypeAsync<BadRequestException>();
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
