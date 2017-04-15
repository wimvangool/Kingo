using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class ReadScenario_Fails_IfAssertResponseCallbackIsNull : ReadScenarioTest<object>
    {
        protected override object ExecuteQuery(IMicroProcessorContext context) =>
            new object();

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public override async Task ThenAsync()
        {
            await Result.IsResponseAsync(null);
        }
    }
}
