using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class QueryTestBase_Fails_IfExceptionIsExpectedButAssertCallbackFails : QueryTestBaseTest<object>
    {
        private const string _Message = "Test";

        protected override object ExecuteQuery(IMicroProcessorContext context) =>
             throw new BadRequestException(_Message);

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            var exceptionToThrow = new MetaAssertFailedException(_Message, null);

            try
            {
                await ThenResult().IsExceptionOfTypeAsync<BadRequestException>(e => throw exceptionToThrow);
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}
