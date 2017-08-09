using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class UnitTestBase_Fails_IfAssertEventStreamIsNull : UnitTestBaseTest<object>
    {
        protected override object WhenMessageIsHandled() =>
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
