using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageHandlerTestBase_Fails_IfAssertEventStreamIsNull : MessageHandlerTestBaseTest<object>
    {
        protected override object When() =>
            new object();

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public override async Task ThenAsync()
        {
            try
            {
                await ThenResult().IsEventStreamAsync(0, stream =>
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
