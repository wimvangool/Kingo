using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageHandlerTestBase_Succeeds_IfStreamIsCorrectlyAssumedEmpty : MessageHandlerTestBaseTest<object>
    {
        protected override object When() =>
            new object();

        [TestMethod]
        public override async Task ThenAsync()
        {
            try
            {
                await ThenResult().IsEventStreamAsync(0);
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
