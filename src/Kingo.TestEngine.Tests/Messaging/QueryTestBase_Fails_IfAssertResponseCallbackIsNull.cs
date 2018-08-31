using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class QueryTestBase_Fails_IfAssertResponseCallbackIsNull : QueryTestBaseTest<object>
    {
        protected override object ExecuteQuery(IMicroProcessorContext context) =>
            new object();

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public override async Task ThenAsync()
        {
            await ThenResult().IsResponseAsync(null);
        }
    }
}
