using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Fails_IfAssertEventStreamIsNull : WriteScenarioTest<object>
    {
        protected override object When() =>
            new object();

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public override async Task ThenAsync()
        {
            try
            {
                await Result.IsEventStreamAsync(0, stream =>
                {
                    AssertEvent<object>(null, 0, @event => { });
                });
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
