using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class ReadScenario_Fails_IfAssertResponseCallbackFails : ReadScenarioTest<object>
    {
        private const string _Message = "Test";

        protected override object ExecuteQuery(IMicroProcessorContext context) =>
            new object();

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            var exceptionToThrow = new MetaAssertFailedException(_Message, null);

            try
            {
                await Result.IsResponseAsync(message =>
                {
                    throw exceptionToThrow;
                });
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
