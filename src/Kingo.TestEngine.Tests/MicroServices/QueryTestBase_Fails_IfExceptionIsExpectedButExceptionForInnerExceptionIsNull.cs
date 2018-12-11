using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class QueryTestBase_Fails_IfExceptionIsExpectedButExceptionForInnerExceptionIsNull : QueryTestBaseTest<object>
    {
        private const string _Message = "Test";

        protected override object ExecuteQuery(IMicroProcessorContext context) =>
             throw new BadRequestException(_Message, new Exception(_Message));

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public override async Task ThenAsync()
        {            
            try
            {
                await ThenResult().IsExceptionOfTypeAsync<BadRequestException>(e =>
                {
                    AssertInnerExceptionIsOfType<Exception>(null, innerException => { });
                });
            }            
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}
