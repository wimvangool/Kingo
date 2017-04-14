using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Fails_IfAssertEventIndexIsOutOfRange : WriteScenarioTest<object>
    {
        protected override object When() =>
            new object();

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public override async Task ThenAsync()
        {
            try
            {
                await Result.IsEventStreamAsync(0, stream =>
                {
                    AssertEvent<object>(stream, 0, @event => { });
                });
            }
            catch (IndexOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("There is no element at index '0' (Count = 0)."));
                throw;
            }
        }        
    }
}
