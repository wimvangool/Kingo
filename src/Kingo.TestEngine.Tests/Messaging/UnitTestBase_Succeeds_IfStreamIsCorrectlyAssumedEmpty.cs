using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class UnitTestBase_Succeeds_IfStreamIsCorrectlyAssumedEmpty : UnitTestBaseTest<object>
    {
        protected override object WhenMessageIsHandled() =>
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
