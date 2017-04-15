using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class ReadScenario_Succeeds_IfCorrectResponseIsExpected : ReadScenarioTest<object>
    {
        private const string _Message = "Test";
        private readonly object _responseMessage = new object();

        protected override object ExecuteQuery(IMicroProcessorContext context) =>
            _responseMessage;

        [TestMethod]        
        public override async Task ThenAsync()
        {            
            try
            {
                await Result.IsResponseAsync(message =>
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
