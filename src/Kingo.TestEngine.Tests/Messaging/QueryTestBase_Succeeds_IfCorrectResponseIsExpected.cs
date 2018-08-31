using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class QueryTestBase_Succeeds_IfCorrectResponseIsExpected : QueryTestBaseTest<object>
    {        
        private readonly object _responseMessage = new object();

        protected override object ExecuteQuery(IMicroProcessorContext context) =>
            _responseMessage;

        [TestMethod]        
        public override async Task ThenAsync()
        {            
            try
            {
                await ThenResult().IsResponseAsync(message =>
                {
                    Assert.AreSame(_responseMessage, message);
                });
            }            
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}
