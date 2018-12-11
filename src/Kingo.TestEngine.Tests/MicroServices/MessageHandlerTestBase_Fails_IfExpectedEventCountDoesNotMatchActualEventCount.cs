using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageHandlerTestBase_Fails_IfExpectedEventCountDoesNotMatchActualEventCount : MessageHandlerTestBaseTest<object>
    {
        protected override object When() =>
            new object();

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            try
            {
                await ThenResult().IsEventStreamAsync(1);
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("The number of expected events (1) does not match the actual amount of published events (0).", exception.Message);
                throw;
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
