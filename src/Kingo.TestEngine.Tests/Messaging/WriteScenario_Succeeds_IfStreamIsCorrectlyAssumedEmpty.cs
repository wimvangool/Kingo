using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Succeeds_IfStreamIsCorrectlyAssumedEmpty : WriteScenarioTest<object>
    {
        protected override object When() =>
            new object();

        [TestMethod]
        public override async Task ThenAsync()
        {
            try
            {
                await Result.IsEventStreamAsync(0);
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
